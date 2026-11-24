namespace ATTENDANCE_AUTOMAILER.Sql
{
    /// <summary>
    /// The only SQL used by the batch. Ported from frmTimekeeper.aspx.vb
    /// (Get_Final_Attd_query, the missing punch query and the consecutive days query).
    ///
    /// Bind variables (System.Data.OracleClient binds by name):
    ///   :p_startdate    dd-MM-yyyy  attendance day
    ///   :p_enddate      dd-MM-yyyy  attendance day (streak query)
    ///   :p_locationcode company / location code
    ///   :p_cutoff_hour  attendance day rollover hour
    ///   :p_gap_hours    In punches closer than this belong to the same segment
    ///   :p_min_days     minimum streak length for the consecutive days query
    ///
    /// Differences from the web page, and why:
    ///  1. The gatepass lookup uses a ROW_NUMBER() CTE instead of OUTER APPLY + FETCH FIRST,
    ///     because OUTER APPLY / FETCH FIRST are not valid on the Oracle versions in use.
    ///  2. Every bind is repeated instead of being built dynamically, so the SQL text is constant
    ///     and the statement can be parsed once per session.
    ///  3. Department / vendor filters are applied in C# so the SQL text does not change.
    ///  4. The streak length is :p_min_days instead of a hard-coded 10.
    /// </summary>
    public static class SqlQueries
    {
        /// <summary>
        /// CTE chain of Get_Final_Attd_query, verbatim apart from the gatepass lookup.
        /// Ends with the CTEs final_attd, cgp_latest and ca_names so callers can join them.
        /// </summary>
        private const string FinalAttdCte = @"
WITH base AS (
    SELECT
        ced.ced_company_code AS company_code,
        ced.ced_dept_code    AS dept_code,
        ced.ced_vendor_code  AS vendor_code,
        a.cpd_card_no,
        a.cpd_person_no,
        ced.ced_firstname || ' ' || ced.ced_lastname AS worker_name,
        a.cpd_inout_flag     AS inout_flag,
        CAST(TRUNC(a.cpd_punch_date) + (a.cpd_punch_time - TRUNC(a.cpd_punch_time)) AS TIMESTAMP) AS punch_ts,
        CAST(
          TRUNC(
            CAST(TRUNC(a.cpd_punch_date) + (a.cpd_punch_time - TRUNC(a.cpd_punch_time)) AS TIMESTAMP)
            - NUMTODSINTERVAL(:p_cutoff_hour, 'HOUR')
          ) AS DATE
        ) AS att_day
    FROM hrace.t_card_punchdata a
    JOIN hrace.t_cemp_details ced
      ON ced.ced_safety_pass_no = a.cpd_person_no
    WHERE ced.ced_company_code = :p_locationcode
      AND ced.ced_sp_enabled IN ('Y')
      AND a.cpd_inout_flag IN ('I','O')
      AND a.cpd_punch_date >= TO_DATE(:p_startdate, 'DD-MM-YYYY')
      AND a.cpd_punch_date <  TO_DATE(:p_startdate, 'DD-MM-YYYY') + 2
),
day_ev AS (
    SELECT *
    FROM base
    WHERE att_day IN (
      TO_DATE(:p_startdate, 'DD-MM-YYYY'),
      TO_DATE(:p_startdate, 'DD-MM-YYYY') + 1
    )
),
day_ev_uniq AS (
    SELECT company_code, dept_code, vendor_code,
           cpd_card_no, cpd_person_no, worker_name,
           att_day, inout_flag, punch_ts
    FROM (
        SELECT d.*,
               ROW_NUMBER() OVER (
                   PARTITION BY company_code, dept_code, vendor_code,
                                cpd_card_no, cpd_person_no,
                                att_day, inout_flag, punch_ts
                   ORDER BY punch_ts
               ) rn
        FROM day_ev d
    )
    WHERE rn = 1
),
segments AS (
    SELECT
        m_company_code  AS company_code,
        m_dept_code     AS dept_code,
        m_vendor_code   AS vendor_code,
        m_cpd_card_no   AS cpd_card_no,
        m_cpd_person_no AS cpd_person_no,
        m_worker_name   AS worker_name,
        m_att_day       AS attendance_day,
        m_seq_no        AS seq_no,
        m_in_ts         AS in_ts,
        m_out_ts        AS out_ts,
        ROUND((CAST(NVL(m_out_ts, SYSTIMESTAMP) AS DATE) - CAST(m_in_ts AS DATE)) * 24 * 60) AS worked_minutes,
        CASE WHEN m_out_ts IS NULL THEN 'OPEN' ELSE 'CLOSED' END AS work_status
    FROM (
        SELECT *
        FROM day_ev_uniq
        MATCH_RECOGNIZE (
            PARTITION BY company_code, dept_code, vendor_code,
                         cpd_card_no, cpd_person_no, att_day
            ORDER BY punch_ts
            MEASURES
                FIRST(I.company_code)  AS m_company_code,
                FIRST(I.dept_code)     AS m_dept_code,
                FIRST(I.vendor_code)   AS m_vendor_code,
                FIRST(I.cpd_card_no)   AS m_cpd_card_no,
                FIRST(I.cpd_person_no) AS m_cpd_person_no,
                FIRST(I.worker_name)   AS m_worker_name,
                FIRST(I.att_day)       AS m_att_day,
                MATCH_NUMBER()         AS m_seq_no,
                FIRST(I.punch_ts)      AS m_in_ts,
                LAST(O.punch_ts)       AS m_out_ts
            ONE ROW PER MATCH
            AFTER MATCH SKIP PAST LAST ROW
            PATTERN (I (S|O)*)
            DEFINE
                I AS I.inout_flag = 'I',
                S AS S.inout_flag = 'I' AND S.punch_ts < FIRST(I.punch_ts) + NUMTODSINTERVAL(:p_gap_hours, 'HOUR'),
                O AS O.inout_flag = 'O'
        )
    )
),
segments_w_next AS (
  SELECT
    s.*,
    LEAD(s.in_ts)          OVER (PARTITION BY s.company_code, s.dept_code, s.vendor_code, s.cpd_card_no, s.cpd_person_no ORDER BY s.attendance_day, s.seq_no) AS next_in_ts,
    LEAD(s.out_ts)         OVER (PARTITION BY s.company_code, s.dept_code, s.vendor_code, s.cpd_card_no, s.cpd_person_no ORDER BY s.attendance_day, s.seq_no) AS next_out_ts,
    LEAD(s.worked_minutes) OVER (PARTITION BY s.company_code, s.dept_code, s.vendor_code, s.cpd_card_no, s.cpd_person_no ORDER BY s.attendance_day, s.seq_no) AS next_worked_minutes,
    COUNT(*)               OVER (PARTITION BY s.company_code, s.dept_code, s.vendor_code, s.cpd_card_no, s.cpd_person_no) AS seg_count
  FROM segments s
),
next_day_cutoff_check AS (
  SELECT
    sw.*,
    (CAST(sw.attendance_day + 1 AS TIMESTAMP) + NUMTODSINTERVAL(:p_cutoff_hour, 'HOUR')) AS next_day_cutoff_ts,
    (
      SELECT MIN(b.punch_ts)
      FROM base b
      WHERE b.company_code  = sw.company_code
        AND b.cpd_card_no   = sw.cpd_card_no
        AND b.cpd_person_no = sw.cpd_person_no
        AND b.inout_flag    = 'I'
        AND b.punch_ts     >= (CAST(sw.attendance_day + 1 AS TIMESTAMP) + NUMTODSINTERVAL(:p_cutoff_hour, 'HOUR'))
    ) AS first_in_after_cutoff_ts,
    (
      SELECT MIN(b2.punch_ts)
      FROM base b2
      WHERE b2.company_code  = sw.company_code
        AND b2.cpd_card_no   = sw.cpd_card_no
        AND b2.cpd_person_no = sw.cpd_person_no
        AND b2.inout_flag    = 'O'
        AND b2.punch_ts     >= (CAST(sw.attendance_day + 1 AS TIMESTAMP) + NUMTODSINTERVAL(:p_cutoff_hour, 'HOUR'))
        AND b2.punch_ts     <  NVL((
                                   SELECT MIN(b3.punch_ts)
                                   FROM base b3
                                   WHERE b3.company_code  = sw.company_code
                                     AND b3.cpd_card_no   = sw.cpd_card_no
                                     AND b3.cpd_person_no = sw.cpd_person_no
                                     AND b3.inout_flag    = 'I'
                                     AND b3.punch_ts     >= (CAST(sw.attendance_day + 1 AS TIMESTAMP) + NUMTODSINTERVAL(:p_cutoff_hour, 'HOUR'))
                                 ), (CAST(sw.attendance_day + 2 AS TIMESTAMP) + NUMTODSINTERVAL(:p_cutoff_hour, 'HOUR')))
    ) AS fallback_next_out_ts,
    CASE
      WHEN sw.next_out_ts IS NULL THEN 'N'
      WHEN EXISTS (
             SELECT 1
             FROM base b
             WHERE b.company_code  = sw.company_code
               AND b.cpd_card_no   = sw.cpd_card_no
               AND b.cpd_person_no = sw.cpd_person_no
               AND b.inout_flag    = 'I'
               AND b.punch_ts     >= (CAST(sw.attendance_day + 1 AS TIMESTAMP) + NUMTODSINTERVAL(:p_cutoff_hour, 'HOUR'))
               AND b.punch_ts      <  sw.next_out_ts
           )
      THEN 'Y' ELSE 'N'
    END AS has_in_after_cutoff_before_next_out
  FROM segments_w_next sw
),
merge_marks AS (
  SELECT
    ncc.*,
    CASE
      WHEN ncc.work_status = 'CLOSED'
       AND ncc.seg_count > 1
       AND ncc.next_in_ts IS NOT NULL
       AND ncc.next_out_ts IS NOT NULL
       AND ncc.next_worked_minutes < 4*60
      THEN 'Y' ELSE 'N'
    END AS merge_with_next,
    CASE
      WHEN ncc.work_status = 'OPEN'
       AND ncc.seg_count > 1
       AND ncc.next_in_ts IS NOT NULL
       AND ncc.next_out_ts IS NULL
      THEN 'Y' ELSE 'N'
    END AS merge_both_open
  FROM next_day_cutoff_check ncc
),
retro_close AS (
  SELECT
    mm.*,
    CASE
      WHEN mm.work_status = 'OPEN'
       AND ( (mm.next_out_ts IS NOT NULL AND mm.has_in_after_cutoff_before_next_out = 'N')
          OR (mm.next_out_ts IS NULL     AND mm.fallback_next_out_ts IS NOT NULL) )
      THEN 'Y' ELSE 'N'
    END AS retro_close_with_next
  FROM merge_marks mm
),
pair_suppression AS (
  SELECT
    rc.*,
    LAG(merge_with_next) OVER (
      PARTITION BY rc.company_code, rc.dept_code, rc.vendor_code,
                   rc.cpd_card_no, rc.cpd_person_no
      ORDER BY rc.attendance_day, rc.seq_no
    ) AS prev_merge_with_next,
    LAG(retro_close_with_next) OVER (
      PARTITION BY rc.company_code, rc.dept_code, rc.vendor_code,
                   rc.cpd_card_no, rc.cpd_person_no
      ORDER BY rc.attendance_day, rc.seq_no
    ) AS prev_retro_close_with_next,
    LAG(merge_both_open) OVER (
      PARTITION BY rc.company_code, rc.dept_code, rc.vendor_code,
                   rc.cpd_card_no, rc.cpd_person_no
      ORDER BY rc.attendance_day, rc.seq_no
    ) AS prev_merge_both_open
  FROM retro_close rc
),
display_rows AS (
  SELECT
    ps.*,
    COALESCE(ps.next_out_ts, ps.fallback_next_out_ts) AS out_to_use,
    CASE
      WHEN ps.merge_with_next = 'Y'
        THEN ps.worked_minutes + NVL(ps.next_worked_minutes, 0)
      WHEN ps.retro_close_with_next = 'Y'
        THEN ROUND( (CAST(COALESCE(ps.next_out_ts, ps.fallback_next_out_ts) AS DATE) - CAST(ps.in_ts AS DATE)) * 24 * 60 )
      WHEN ps.merge_both_open = 'Y'
        THEN ROUND( (CAST(SYSTIMESTAMP AS DATE) - CAST(ps.in_ts AS DATE)) * 24 * 60 )
      ELSE ps.worked_minutes
    END AS display_minutes,
    CASE
      WHEN ps.retro_close_with_next = 'Y' THEN COALESCE(ps.next_out_ts, ps.fallback_next_out_ts)
      WHEN ps.merge_with_next       = 'Y' THEN ps.next_out_ts
      WHEN ps.merge_both_open       = 'Y' THEN NULL
      ELSE ps.out_ts
    END AS out_ts_display,
    ps.in_ts AS in_ts_display,
    CASE
      WHEN ps.retro_close_with_next = 'Y' THEN 'CLOSED'
      ELSE ps.work_status
    END AS display_work_status,
    CASE
      WHEN NVL(ps.prev_merge_with_next, 'N') = 'Y'
        OR NVL(ps.prev_retro_close_with_next, 'N') = 'Y'
        OR NVL(ps.prev_merge_both_open, 'N') = 'Y'
      THEN 'Y' ELSE 'N'
    END AS suppress_row
  FROM pair_suppression ps
),
final_attd AS (
  SELECT
    company_code, dept_code, vendor_code,
    cpd_card_no, cpd_person_no, worker_name,
    attendance_day, seq_no,
    in_ts, out_ts,
    in_ts_display  AS in_ts_resolved,
    out_ts_display AS out_ts_resolved,
    display_minutes,
    ROUND(display_minutes / 60, 2) AS display_hours,
    LPAD(FLOOR(display_minutes / 60), 4, '0') || ':' || LPAD(MOD(display_minutes, 60), 2, '0') AS display_hhmm,
    display_minutes AS raw_minutes,
    display_work_status AS work_status,
    merge_with_next,
    merge_both_open,
    retro_close_with_next,
    seg_count,
    CASE
        WHEN TO_CHAR(in_ts_display, 'HH24:MI') BETWEEN '05:00' AND '07:00' THEN 'A'
        WHEN TO_CHAR(in_ts_display, 'HH24:MI') BETWEEN '13:00' AND '15:00' THEN 'B'
        WHEN TO_CHAR(in_ts_display, 'HH24:MI') BETWEEN '21:00' AND '23:00' THEN 'C'
        WHEN TO_CHAR(in_ts_display, 'HH24:MI') BETWEEN '07:01' AND '10:00' THEN 'G'
        ELSE 'U'
    END AS shift_category
  FROM display_rows
  WHERE suppress_row = 'N'
    AND attendance_day = TO_DATE(:p_startdate, 'DD-MM-YYYY')
),
cgp_latest AS (
    SELECT cgp_safety_pass_no, cgp_comp_code, cgp_wo_number, cgp_vendor_code
    FROM (
        SELECT g.cgp_safety_pass_no, g.cgp_comp_code, g.cgp_wo_number, g.cgp_vendor_code,
               ROW_NUMBER() OVER (PARTITION BY g.cgp_safety_pass_no
                                  ORDER BY NVL(g.cgp_release_dt, g.cgp_gp_issued_on) DESC) AS rn
        FROM hrace.t_cemp_gatepass g
        WHERE g.cgp_comp_code = :p_locationcode
    )
    WHERE rn = 1
),
ca_names AS (
    SELECT w.wod_comp_code, w.wod_wo_number,
           LISTAGG(e.ema_ename, ' and ') WITHIN GROUP (ORDER BY e.ema_ename) AS owner_and_admin
    FROM hrace.t_workorder_details w
    JOIN hrace.v_empl_all e
      ON e.ema_perno IN (w.wod_cont_owner, w.wod_cont_admin)
    WHERE w.wod_comp_code = :p_locationcode
    GROUP BY w.wod_comp_code, w.wod_wo_number
)";

        /// <summary>
        /// Resolved in/out segments of the attendance day, every shift. Shift filtering and the
        /// violation rules are applied in Program.cs.
        /// </summary>
        public static string AttendanceDetails
        {
            get
            {
                return FinalAttdCte + @"
SELECT
    a.company_code             AS location_code,
    a.dept_code                AS dept_code,
    (SELECT cdp_dept_code || '-' || cdp_dept_name
       FROM hrace.t_cnt_dept_master
      WHERE cdp_comp_code = :p_locationcode
        AND cdp_dept_code = a.dept_code) AS department_name,
    a.cpd_person_no            AS safety_pass_no,
    a.cpd_card_no              AS card_no,
    a.worker_name              AS worker_name,
    NVL(cgp.cgp_vendor_code, a.vendor_code) AS vendor_code,
    (SELECT vdt_vendor_name
       FROM hrace.t_vendor_details
      WHERE vdt_company_code = :p_locationcode
        AND vdt_vendor_code  = NVL(cgp.cgp_vendor_code, a.vendor_code)) AS vendor_name,
    (SELECT vmd_nature_of_work
       FROM hrace.t_vendor_misc_details
      WHERE vmd_comp_code   = :p_locationcode
        AND vmd_vendor_code = NVL(cgp.cgp_vendor_code, a.vendor_code)) AS nature_of_work,
    cgp.cgp_wo_number          AS wo_no,
    ca_names.owner_and_admin   AS ca_co_name,
    a.attendance_day           AS attendance_day,
    a.seq_no                   AS seq_no,
    a.shift_category           AS shift_code,
    a.in_ts_resolved           AS in_time,
    a.out_ts_resolved          AS out_time,
    a.display_minutes          AS display_minutes,
    a.display_hours            AS display_hours,
    a.display_hhmm             AS total_hours,
    a.work_status              AS work_status,
    a.seg_count                AS seg_count
FROM final_attd a
LEFT JOIN cgp_latest cgp
       ON cgp.cgp_safety_pass_no = a.cpd_person_no
      AND cgp.cgp_comp_code      = a.company_code
LEFT JOIN ca_names
       ON ca_names.wod_comp_code = a.company_code
      AND ca_names.wod_wo_number = cgp.cgp_wo_number
ORDER BY a.cpd_person_no, a.seq_no";
            }
        }

        /// <summary>
        /// Workers whose first punch of the attendance day is an "Out" punch, i.e. the In punch is
        /// missing. first_punch holds one row per worker, so no GROUP BY is needed.
        /// </summary>
        public static string MissingInPunches
        {
            get
            {
                return @"
WITH p AS (
    SELECT
        ced.ced_company_code AS company_code,
        ced.ced_dept_code    AS dept_code,
        ced.ced_vendor_code  AS vendor_code,
        a.cpd_card_no,
        a.cpd_person_no,
        ced.ced_firstname || ' ' || ced.ced_lastname AS worker_name,
        a.cpd_inout_flag AS inout_flag,
        CAST(TRUNC(a.cpd_punch_date) + (a.cpd_punch_time - TRUNC(a.cpd_punch_time)) AS TIMESTAMP) AS punch_ts,
        CAST(
          TRUNC(
            CAST(TRUNC(a.cpd_punch_date) + (a.cpd_punch_time - TRUNC(a.cpd_punch_time)) AS TIMESTAMP)
            - NUMTODSINTERVAL(:p_cutoff_hour, 'HOUR')
          ) AS DATE
        ) AS att_day
    FROM hrace.t_card_punchdata a
    JOIN hrace.t_cemp_details ced
      ON ced.ced_safety_pass_no = a.cpd_person_no
    WHERE ced.ced_company_code = :p_locationcode
      AND ced.ced_sp_enabled IN ('Y')
      AND a.cpd_inout_flag IN ('I','O')
      AND a.cpd_punch_date >= TO_DATE(:p_startdate, 'DD-MM-YYYY')
      AND a.cpd_punch_date <  TO_DATE(:p_startdate, 'DD-MM-YYYY') + 2
),
d AS (
    SELECT * FROM p WHERE att_day = TO_DATE(:p_startdate, 'DD-MM-YYYY')
),
ranked AS (
    SELECT d.*,
           ROW_NUMBER() OVER (PARTITION BY d.cpd_card_no, d.cpd_person_no ORDER BY d.punch_ts) AS rn
    FROM d
),
first_punch AS (
    SELECT company_code, dept_code, vendor_code, cpd_card_no, cpd_person_no, worker_name, punch_ts
    FROM ranked
    WHERE rn = 1
      AND inout_flag = 'O'
),
cgp_latest AS (
    SELECT cgp_safety_pass_no, cgp_comp_code, cgp_wo_number, cgp_vendor_code
    FROM (
        SELECT g.cgp_safety_pass_no, g.cgp_comp_code, g.cgp_wo_number, g.cgp_vendor_code,
               ROW_NUMBER() OVER (PARTITION BY g.cgp_safety_pass_no
                                  ORDER BY NVL(g.cgp_release_dt, g.cgp_gp_issued_on) DESC) AS rn
        FROM hrace.t_cemp_gatepass g
        WHERE g.cgp_comp_code = :p_locationcode
    )
    WHERE rn = 1
)
SELECT
    f.company_code AS location_code,
    f.dept_code    AS dept_code,
    (SELECT cdp_dept_code || '-' || cdp_dept_name
       FROM hrace.t_cnt_dept_master
      WHERE cdp_comp_code = :p_locationcode
        AND cdp_dept_code = f.dept_code) AS department_name,
    f.cpd_person_no AS safety_pass_no,
    f.cpd_card_no   AS card_no,
    f.worker_name   AS worker_name,
    NVL(cgp.cgp_vendor_code, f.vendor_code) AS vendor_code,
    (SELECT vdt_vendor_name
       FROM hrace.t_vendor_details
      WHERE vdt_company_code = :p_locationcode
        AND vdt_vendor_code  = NVL(cgp.cgp_vendor_code, f.vendor_code)) AS vendor_name,
    cgp.cgp_wo_number AS wo_no,
    f.punch_ts        AS first_punch_time
FROM first_punch f
LEFT JOIN cgp_latest cgp
       ON cgp.cgp_safety_pass_no = f.cpd_person_no
      AND cgp.cgp_comp_code      = f.company_code
ORDER BY f.cpd_person_no";
            }
        }

        /// <summary>
        /// Gap-and-islands over daily punches (port of GetConsecutive10DaysDetailsQuery):
        /// streaks ending on the attendance day with at least :p_min_days days.
        /// The window is the same 29 days used by the web page.
        /// </summary>
        public static string ContinuousStreaks
        {
            get
            {
                return @"
WITH punch_dates AS (
    SELECT DISTINCT a.cpd_person_no, TRUNC(a.cpd_punch_date) AS work_date
    FROM hrace.t_card_punchdata a
    JOIN hrace.t_cemp_details ced
      ON ced.ced_safety_pass_no = a.cpd_person_no
     AND ced.ced_sp_enabled  IN ('Y','N')
     AND ced.ced_company_code = :p_locationcode
    WHERE a.cpd_inout_flag = 'I'
      AND TRUNC(a.cpd_punch_date) BETWEEN TO_DATE(:p_enddate, 'DD-MM-YYYY') - 29
                                     AND TO_DATE(:p_enddate, 'DD-MM-YYYY')
),
marked AS (
    SELECT
        cpd_person_no,
        work_date,
        CASE
            WHEN LAG(work_date) OVER (PARTITION BY cpd_person_no ORDER BY work_date) = work_date - 1
            THEN 0 ELSE 1
        END AS is_break
    FROM punch_dates
),
grp AS (
    SELECT
        cpd_person_no,
        work_date,
        SUM(is_break) OVER (PARTITION BY cpd_person_no ORDER BY work_date) AS grp
    FROM marked
),
streaks AS (
    SELECT cpd_person_no,
           MIN(work_date) AS start_date,
           MAX(work_date) AS end_date,
           COUNT(*)       AS consecutive_days
    FROM grp
    GROUP BY cpd_person_no, grp
),
valid_streaks AS (
    SELECT * FROM streaks
    WHERE consecutive_days >= :p_min_days
      AND end_date = TO_DATE(:p_enddate, 'DD-MM-YYYY')
)
SELECT
    cd.ced_company_code AS location_code,
    cd.ced_dept_code    AS dept_code,
    (SELECT cdp_dept_code || '-' || cdp_dept_name
       FROM hrace.t_cnt_dept_master
      WHERE cdp_comp_code = :p_locationcode
        AND cdp_dept_code = cd.ced_dept_code) AS department_name,
    vs.cpd_person_no    AS safety_pass_no,
    cd.ced_firstname || ' ' || cd.ced_lastname AS worker_name,
    cd.ced_vendor_code  AS vendor_code,
    (SELECT vdt_vendor_name
       FROM hrace.t_vendor_details
      WHERE vdt_company_code = :p_locationcode
        AND vdt_vendor_code  = cd.ced_vendor_code) AS vendor_name,
    vs.start_date       AS start_date,
    vs.end_date         AS end_date,
    vs.consecutive_days AS consecutive_days
FROM valid_streaks vs
JOIN hrace.t_cemp_details cd
  ON cd.ced_safety_pass_no = vs.cpd_person_no
 AND cd.ced_sp_enabled  IN ('Y','N')
 AND cd.ced_company_code = :p_locationcode
ORDER BY vs.cpd_person_no";
            }
        }
    }
}
