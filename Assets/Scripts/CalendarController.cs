using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;

public class CalendarController : MonoBehaviour
{
    public enum CalendarMode
    {
        SelectHide = 0,
        SelectShow = 1
    }

    private enum MonthNames
    {   January = 1,
        February = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12
    }

    public GameObject _calendarPanel;
    public TMP_Text _yearNumText;
    public TMP_Text _monthNumText;

    public Button _item;
    public RectTransform _itemContainer;
    public CanvasGroup _itemCanvasGroup;

    public Color32 _todayDateColor;
    public Color32 _selectDateColor;
    public Color32 _highlightDateColor;
    public CalendarMode _calendarMode = CalendarMode.SelectHide;

    public List<Button> _dateItems = new List<Button>();
    const int _totalDateNum = 43;

    private DateTime _dateTime;
    private TMP_Text currentSelectDateText;
    private TMP_Text previousSelectDateText;
    private List<int> appointmentDayList = new List<int>();

    public Action<DateTime> dateTrigger;
    private Func<DateTime, UniTask<List<AppointmentData>>> getAppointmentTriggerCallback;

    public async UniTask init(Func<DateTime, UniTask<List<AppointmentData>>> getAppointmentTrigger)
    {
        for (int i = 0; i < _totalDateNum; i++)
        {
            Button item = Instantiate(_item, _itemContainer);
            item.name = "Item" + i.ToString();

            _dateItems.Add(item);
        }

        getAppointmentTriggerCallback = getAppointmentTrigger;

        ResetDateToToday();

        _itemCanvasGroup.alpha = 0;
        _itemCanvasGroup.interactable = false;
        _itemCanvasGroup.blocksRaycasts = false;

        await UniTask.CompletedTask;
    }

    /// <summary>
    /// Show the calendar for the current month and year
    /// </summary>
    async UniTask CreateCalendar()
    {
        if (getAppointmentTriggerCallback != null)
        {
            List<AppointmentData> appointmentDataList = await getAppointmentTriggerCallback(_dateTime);
            Debug.Log("Appointments in month: " + appointmentDataList.Count);
            appointmentDayList.Clear();

            for (int i = 0; i < appointmentDataList.Count; i++)
            {
                int day = appointmentDataList[i].Date.ToDateTime().Day;

                if (appointmentDayList.Contains(day) == false)
                {
                    appointmentDayList.Add(day);
                }
            }
        }

        bool matchToday = MatchTodayDate(_dateTime);

        DateTime firstDay = _dateTime.AddDays(-(_dateTime.Day - 1));
        int index = GetDays(firstDay.DayOfWeek);

        int date = 0;
        for (int i = 0; i < _totalDateNum; i++)
        {
            int dayIndex = i;

            _dateItems[dayIndex].onClick.RemoveAllListeners();
            TMP_Text label = _dateItems[dayIndex].GetComponentInChildren<TMP_Text>();
            label.color = Color.black;
            Image highlight = _dateItems[dayIndex].GetComponentInChildren<Image>();
            CanvasGroup canvasGroup = _dateItems[dayIndex].GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            if (dayIndex >= index)
            {
                DateTime thatDay = firstDay.AddDays(date);

                if (matchToday && _dateTime.Day == dayIndex)
                {
                    label.color = _todayDateColor;
                }
                else
                {
                    label.color = Color.black;
                }

                if (appointmentDayList.Contains(thatDay.Day) && _calendarMode == CalendarMode.SelectShow)
                {
                    highlight.color = _highlightDateColor;
                }
                else
                {
                    highlight.color = Color.white;
                }

                if (thatDay.Month == firstDay.Month)
                {
                    canvasGroup.alpha = 1;
                    canvasGroup.interactable = true;
                    canvasGroup.blocksRaycasts = true;

                    int currentDay = date + 1;
                    DateTime currentDate = new DateTime(_dateTime.Year, _dateTime.Month, currentDay);

                    _dateItems[dayIndex].onClick.AddListener(() => OnDateItemClick(dayIndex, currentDate));
                    label.text = currentDay.ToString();
                    date++;
                }
            }
        }
        _yearNumText.text = _dateTime.Year.ToString();
        _monthNumText.text = ((MonthNames)_dateTime.Month).ToString();
    }

    /// <summary>
    /// Get the day based on the day of the week
    /// </summary>
    /// <param name="day"></param>
    /// <returns></returns>
    int GetDays(DayOfWeek day)
    {
        switch (day)
        {
            case DayOfWeek.Monday: return 1;
            case DayOfWeek.Tuesday: return 2;
            case DayOfWeek.Wednesday: return 3;
            case DayOfWeek.Thursday: return 4;
            case DayOfWeek.Friday: return 5;
            case DayOfWeek.Saturday: return 6;
            case DayOfWeek.Sunday: return 0;
        }

        return 0;
    }

    /// <summary>
    /// Show previous year
    /// </summary>
    public void YearPrev()
    {
        _dateTime = _dateTime.AddYears(-1);
        CreateCalendar().Forget();
    }

    /// <summary>
    /// Show next year
    /// </summary>
    public void YearNext()
    {
        _dateTime = _dateTime.AddYears(1);
        CreateCalendar().Forget();
    }

    /// <summary>
    /// Show previous month
    /// </summary>
    public void MonthPrev()
    {
        _dateTime = _dateTime.AddMonths(-1);
        CreateCalendar().Forget();
    }

    /// <summary>
    /// Show next month
    /// </summary>
    public void MonthNext()
    {
        _dateTime = _dateTime.AddMonths(1);
        CreateCalendar().Forget();
    }

    /// <summary>
    /// Resset date to today
    /// </summary>
    private void ResetDateToToday()
    {
        _dateTime = DateTime.Now;
    }

    /// <summary>
    /// Show the calendar at the specified location with the specified scale
    /// </summary>
    /// <param name="location"></param>
    /// <param name="scale"></param>
    public void ShowCalendar(Vector3 location, Vector3 scale)
    {
        ResetDateToToday();
        CreateCalendar().Forget();

        _itemCanvasGroup.alpha = 1;
        _itemCanvasGroup.interactable = true;
        _itemCanvasGroup.blocksRaycasts = true;
        transform.position = location;
        _itemCanvasGroup.transform.localScale = scale;
    }

    /// <summary>
    /// hide the calendar
    /// </summary>
    public void HideCalendar()
    {
        _itemCanvasGroup.alpha = 0;
        _itemCanvasGroup.interactable = false;
        _itemCanvasGroup.blocksRaycasts = false;
    }

    public void OnDateItemClick(int dayObject, DateTime day)
    {
        switch (_calendarMode)
        {
            case CalendarMode.SelectHide:
                HideCalendar();
                break;
            case CalendarMode.SelectShow:
                if (currentSelectDateText != null)
                {
                    previousSelectDateText = currentSelectDateText;
                }

                currentSelectDateText = _dateItems[dayObject].GetComponentInChildren<TMP_Text>();
                currentSelectDateText.color = _selectDateColor;
                break;
            default:
                break;
        }

        if (dateTrigger != null)
        {
            dateTrigger(day);
        }
    }

    /// <summary>
    /// Compare the given date with today's date
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    private bool MatchTodayDate(DateTime date)
    {
        return date.Month == DateTime.Now.Month && date.Year == DateTime.Now.Year;
    }
}
