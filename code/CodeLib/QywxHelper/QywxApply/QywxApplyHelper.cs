using System;
// ReSharper disable once RedundantUsingDirective
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeLib01;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static QywxHelper.QywxApply.QywxApplyHelper;

#pragma warning disable CS0219,CS8600,CS8602,CS8603,CS8604,CS8618,CS8619,CS8625,CS8714

namespace QywxHelper.QywxApply;

/// <summary>
/// 企业微信审批助手<br/>
/// https://developer.work.weixin.qq.com/document/path/91983
/// </summary>
public class QywxApplyHelper
{
    /// <summary>
    /// 附1 文本控件（control参数为Text）
    /// </summary>
    public const string CONTROL_TEXT = "Text";

    /// <summary>
    /// 附1 多行文本控件（control参数为Textarea）
    /// </summary>
    public const string CONTROL_TEXTAREA = "Textarea";

    /// <summary>
    /// 附2 数字控件（control参数为Number）
    /// </summary>
    public const string CONTROL_NUMBER = "Number";

    /// <summary>
    /// 附3 金额控件（control参数为Money）
    /// </summary>
    public const string CONTROL_MONEY = "Money";

    /// <summary>
    /// 附4 日期/日期+时间控件（control参数为Date）
    /// </summary>
    public const string CONTROL_DATE = "Date";

    /// <summary>
    ///附5 单选/多选控件（control参数为Selector）
    /// </summary>
    public const string CONTROL_SELECTOR = "Selector";

    /// <summary>
    /// 附6 成员控件（control参数为Contact，且value参数为members）<br/>
    /// 附7 部门控件（control参数为Contact，且value参数为departments）
    /// </summary>
    public const string CONTROL_CONTACT = "Contact";

    /// <summary>
    /// 附9 附件控件（control参数为File）
    /// </summary>
    public const string CONTROL_FILE = "File";

    /// <summary>
    /// 附10 明细控件（control参数为Table）
    /// </summary>
    public const string CONTROL_TABLE = "Table";

    /// <summary>
    /// 附11 假勤组件-请假组件（control参数为Vacation）
    /// </summary>
    public const string CONTROL_VACATION = "Vacation";

    /// <summary>
    /// 附12 假勤组件-出差/外出/加班组件（control参数为Attendance）
    /// </summary>
    public const string CONTROL_ATTENDANCE = "Attendance";

    /// <summary>
    /// 附13 补卡组件（control参数为PunchCorrection）
    /// </summary>
    public const string CONTROL_PUNCH_CORRECTION = "PunchCorrection";

    /// <summary>
    /// 附14 时长组件（control参数为DateRange）
    /// </summary>
    public const string CONTROL_DATE_RANGE = "DateRange";

    /// <summary>
    /// 附15 位置控件（control参数为Location）
    /// </summary>
    public const string CONTROL_LOCATION = "Location";

    /// <summary>
    /// 附16 关联审批单控件（control参数为RelatedApproval）
    /// </summary>
    public const string CONTROL_RELATED_APPROVAL = "RelatedApproval";

    /// <summary>
    /// 附17 公式控件（control参数为Formula）
    /// </summary>
    public const string CONTROL_FORMULA = "Formula";


    #region 封装提取值的方法

    /// <summary>
    /// 根据控件类型自动获取第一个值
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    public static string? GetValue(JToken? control)
    {
        if (control is null) return null;
        // 控件类型
        string control_type = control["control"]?.ToModel<string>();
        switch (control_type)
        {
            case CONTROL_TEXT:
            case CONTROL_TEXTAREA:
                return GetText(control);
            case CONTROL_NUMBER:
                return GetNumber(control);
            case CONTROL_MONEY:
                return GetMoney(control);
            case CONTROL_DATE:
                return GetDateValue(control)?.ToString("yyyy-MM-dd HH:mm:ss");
            case CONTROL_SELECTOR:
                return GetSelectSingle(control);
            case CONTROL_CONTACT:
                return ((object)GetMember(control) ?? GetDepartment(control))?.ToJson();
            case CONTROL_FILE:
                return GetFileId(control);
            case CONTROL_TABLE:
                return ControlsToJObject(control["value"]["children"]?.SelectMany(child1 => child1)).ToJson();
            case CONTROL_VACATION:
                return GetVacation(control)?.ToJson();
            case CONTROL_ATTENDANCE:
                return GetAttendance(control)?.ToJson();
            case CONTROL_PUNCH_CORRECTION:
                return GetPunchCorrection(control)?.ToJson();
            case CONTROL_DATE_RANGE:
                return GetDateRange(control)?.ToJson();
            case CONTROL_LOCATION:
                return GetLocation(control)?.ToJson();
            case CONTROL_RELATED_APPROVAL:
                return GetRelatedApprovals(control).ToJson();
            case CONTROL_FORMULA:
                return GetFormulaValue(control);
        }

        return null;
    }


    /// <summary>
    /// 根据控件类型自动值列表
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    public static List<string?> GetValues(JToken? control)
    {
        if (control is null) return new List<string?>();
        List<string?> ToList(string? value1) => value1 != null ? new List<string?> { value1 } : new List<string?>();

        // 控件类型
        var control_type = control["control"]?.ToModel<string>();
        switch (control_type)
        {
            case CONTROL_TEXT:
            case CONTROL_TEXTAREA:
                return ToList(GetText(control));
            case CONTROL_NUMBER:
                return ToList(GetNumber(control));
            case CONTROL_MONEY:
                return ToList(GetMoney(control));
            case CONTROL_DATE:
                return ToList(GetDateValue(control)?.ToString("yyyy-MM-dd HH:mm:ss"));
            case CONTROL_SELECTOR:
                return GetSelectMulti(control);
            case CONTROL_CONTACT: // 成员/部门
            {
                var members = GetMembers(control);
                if (members.Count > 0) return members.Select(p => p?.ToJson()).ToList();
                return GetDepartments(control).Select(p => p?.ToJson()).ToList();
            }
            case CONTROL_FILE:
                return GetFileIds(control);

            case CONTROL_TABLE:
                return GetTableControls(control).Select(kv => new KeyValuePair<string, string>(kv.Key, GetValue(kv.Value)).ToJson()).ToList();

            case CONTROL_VACATION:
                return ToList(GetVacation(control)?.ToJson());
            case CONTROL_ATTENDANCE:
                return ToList(GetAttendance(control)?.ToJson());
            case CONTROL_PUNCH_CORRECTION:
                return ToList(GetPunchCorrection(control)?.ToJson());
            case CONTROL_DATE_RANGE:
            {
                var range1 = GetDateRange(control);
                return new List<string?>
                {
                    range1?.begin?.ToString("yyyy-MM-dd HH:mm:ss"),
                    range1?.end?.ToString("yyyy-MM-dd HH:mm:ss"),
                };
            }
            case CONTROL_LOCATION:
                return ToList(GetLocation(control)?.ToJson());
            case CONTROL_RELATED_APPROVAL:
                return GetRelatedApprovals(control).Select(p => p.ToJson()).ToList();

            case CONTROL_FORMULA:
                return ToList(GetFormulaValue(control));
        }

        return new List<string?>();
    }

    /// <summary>
    /// 获取类型值
    /// </summary>
    /// <param name="control"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetModel<T>(JToken? control) => (T)GetModel(control, typeof(T));

    private static object GetModel(JToken? control, Type type)
    {
        if (control is null)
            return (type.IsValueType ? Activator.CreateInstance(type) : null);
        object result = null;

        // 控件类型
        string control_type = control["control"]?.ToModel<string>();
        switch (control_type)
        {
            case CONTROL_DATE:
                if (type == typeof(QywxApplyDate))
                    return GetDate(control);
                break;
            case CONTROL_SELECTOR:
                if (type == typeof(QywxApplySelector))
                    return GetSelector(control);
                break;
            case CONTROL_CONTACT: // 成员/部门
            {
                // 成员
                if (type == typeof(QywxApplyMember))
                    return GetMember(control);

                // 部门
                if (type == typeof(QywxApplyDepartment))
                    return GetDepartment(control);

                #region 多个成员

                var members_type = new List<Type>
                {
                    typeof(List<QywxApplyMember>),
                    typeof(IList<QywxApplyMember>),
                    typeof(ICollection<QywxApplyMember>),
                    typeof(IEnumerable<QywxApplyMember>),
                };
                if (members_type.Any(t => type == t))
                    return GetMembers(control);
                if (type == typeof(QywxApplyMember[]))
                    return (GetMembers(control).ToArray());

                #endregion

                #region 多个部门

                var departments_type = new List<Type>
                {
                    typeof(List<QywxApplyDepartment>),
                    typeof(IList<QywxApplyDepartment>),
                    typeof(ICollection<QywxApplyDepartment>),
                    typeof(IEnumerable<QywxApplyDepartment>),
                };
                if (departments_type.Any(t => type == t))
                    return GetDepartments(control);
                if (type == typeof(QywxApplyDepartment[]))
                    return (GetDepartments(control).ToArray());

                #endregion
            }
                break;

            case CONTROL_FILE:
            {
                if (type == typeof(QywxApplyFile))
                    return GetFile(control);

                var types = new List<Type>
                {
                    typeof(List<QywxApplyFile>),
                    typeof(IList<QywxApplyFile>),
                    typeof(ICollection<QywxApplyFile>),
                    typeof(IEnumerable<QywxApplyFile>),
                };
                if (types.Any(t => type == t))
                    return GetFiles(control);
                if (type == typeof(QywxApplyFile[]))
                    return (GetFiles(control).ToArray());
            }
                break;
            case CONTROL_TABLE:
                return ControlsToJObject(GetTableControls(control).Values).ToTypeModel(type);
            case CONTROL_VACATION:
                if (type == typeof(QywxApplyVacation))
                    return GetVacation(control);
                break;
            case CONTROL_ATTENDANCE:
                if (type == typeof(QywxApplyAttendance))
                    return GetAttendance(control);
                break;
            case CONTROL_PUNCH_CORRECTION:
                if (type == typeof(QywxApplyPunchCorrection))
                    return GetPunchCorrection(control);
                break;
            case CONTROL_DATE_RANGE:
                if (type == typeof(QywxApplyDateRange))
                    return GetDateRange(control);
                break;
            case CONTROL_LOCATION:
                if (type == typeof(QywxApplyLocation))
                    return GetLocation(control);
                break;
            case CONTROL_RELATED_APPROVAL:
            {
                // 关联审批单控件
                if (type == typeof(QywxApplyRelatedApproval))
                    return GetRelatedApproval(control);
                var types = new List<Type>
                {
                    typeof(List<QywxApplyRelatedApproval>),
                    typeof(IList<QywxApplyRelatedApproval>),
                    typeof(ICollection<QywxApplyRelatedApproval>),
                    typeof(IEnumerable<QywxApplyRelatedApproval>),
                };
                if (types.Any(t => type == t))
                    return GetRelatedApprovals(control);
                if (type == typeof(QywxApplyRelatedApproval[]))
                    return (GetRelatedApprovals(control).ToArray());
            }
                break;
            case CONTROL_FORMULA:
                if (type == typeof(QywxApplyFormula))
                    return GetFormula(control);
                break;
        }

        if (GetString())
            return result;

        // 动态转换 只对 文本类型 有效
        string value = GetValue(control);
        if (!string.IsNullOrWhiteSpace(value))
        {
            result = new JValue(value).ToTypeModel(type);
            if (result != null)
                return result;
        }

        // 动态转换 只对 选项列表有效
        var values = GetValues(control);
        if (values.Count > 0)
        {
            result = new JValue(values).ToTypeModel(type);
            if (result != null)
                return result;
        }

        return (type.IsValueType ? Activator.CreateInstance(type) : null);

        bool GetString()
        {
            // 2023-7-29 Ciaran 其他类型, 默认按字符串处理
            if (type == typeof(string))
            {
                result = GetValue(control);
                return true;
            }

            var string_types = new List<Type>
            {
                typeof(List<string>),
                typeof(IList<string>),
                typeof(ICollection<string>),
                typeof(IEnumerable<string>),
            };
            if (string_types.Any(t => type == t))
            {
                result = GetValues(control);
                return true;
            }

            if (type == typeof(string[]))
            {
                result = (GetValues(control).ToArray());
                return true;
            }

            return false;
        }
    }

    /// <summary>
    /// 附1 文本/多行文本控件（control参数为Text或Textarea）
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    public static string? GetText(JToken? control) => control["value"]["text"]?.ToModel<string>();

    /// <summary>
    /// 附2 数字控件（control参数为Number）
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    public static string? GetNumber(JToken? control) => control["value"]["new_number"]?.ToModel<string>();

    /// <summary>
    /// 附3 金额控件（control参数为Money）
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    public static string? GetMoney(JToken? control) => control["value"]["new_money"]?.ToModel<string>();

    /// <summary>
    /// 附4 日期/日期+时间控件（control参数为Date）
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    public static QywxApplyDate? GetDate(JToken? control) => control["value"]["date"]?.ToModel<QywxApplyDate>();

    /// <summary>
    /// 附4 日期/日期+时间控件（control参数为Date）
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    public static DateTime? GetDateValue(JToken? control) => GetDate(control)?.datetime;

    /// <summary>
    /// 附5 单选控件（control参数为Selector）
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    public static QywxApplySelector? GetSelector(JToken? control) => control["value"]["selector"]?.ToModel<QywxApplySelector>();

    /// <summary>
    /// 附5 单选控件（control参数为Selector）
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    public static string? GetSelectSingle(JToken? control) => GetSelector(control)?.Value;

    /// <summary>
    /// 附5 多选控件（control参数为Selector）
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    public static List<string> GetSelectMulti(JToken? control) => GetSelector(control)?.Values ?? new List<string>();

    /// <summary>
    /// 附6 成员控件（control参数为Contact，且value参数为members）
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    public static QywxApplyMember? GetMember(JToken? control) => control["value"]["members"]?.FirstOrDefault()?.ToModel<QywxApplyMember?>();

    /// <summary>
    /// 附6 成员控件（control参数为Contact，且value参数为members）
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    public static List<QywxApplyMember?> GetMembers(JToken? control) => control["value"]["members"]?.Select(p => p.ToModel<QywxApplyMember?>()).ToList() ?? new List<QywxApplyMember?>();

    /// <summary>
    /// 附7 部门控件（control参数为Contact，且value参数为departments）
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    public static QywxApplyDepartment? GetDepartment(JToken? control) => control["value"]["departments"]?.FirstOrDefault()?.ToModel<QywxApplyDepartment?>();

    /// <summary>
    /// 附7 部门控件（control参数为Contact，且value参数为departments）
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    public static List<QywxApplyDepartment?> GetDepartments(JToken? control) => control["value"]["departments"]?.Select(p => p.ToModel<QywxApplyDepartment?>()).ToList() ?? new List<QywxApplyDepartment?>();


    /// <summary>
    /// 附9 附件控件（control参数为File）
    /// <br/>单文件id
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    public static string? GetFileId(JToken? control) => control["value"]["files"]?.FirstOrDefault()?["file_id"]?.ToModel<string>();

    /// <summary>
    /// 附9 附件控件（control参数为File）
    /// <br/>多文件id
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    public static List<string> GetFileIds(JToken? control) => (control["value"]["files"] as JArray)?.Select(p => p["file_id"]?.ToModel<string>()).ToList() ?? new List<string>();

    /// <summary>
    /// 附9 附件控件（control参数为File）
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    public static QywxApplyFile? GetFile(JToken? control) => control["value"]["files"]?.FirstOrDefault()?.ToModel<QywxApplyFile>();

    /// <summary>
    /// 附9 附件控件（control参数为File）
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    public static List<QywxApplyFile> GetFiles(JToken? control) => control["value"]["files"]?.ToModel<List<QywxApplyFile>>() ?? new List<QywxApplyFile>();

    /// <summary>
    /// 附10 明细控件（control参数为Table）
    /// <br/>获取表格控件列表
    /// </summary>
    /// <param name="control">明细控件</param>
    /// <returns></returns>
    public static Dictionary<string, JToken?> GetTableControls(JToken? control) =>
        control["value"]["children"]?.SelectMany(child1 => child1["list"]?.Select(item => new KeyValuePair<string, JToken>(item["title"]?.FirstOrDefault(value1 => value1["lang"]?.ToModel<string>() == "zh_CN")?["text"]?.ToModel<string>(), item)))
            .ToDictionary(kv => kv.Key, kv => kv.Value) ?? new Dictionary<string, JToken?>();

    /// <summary>
    /// 附11 假勤组件-请假组件（control参数为Vacation）
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    public static QywxApplyVacation? GetVacation(JToken? control) => control["value"]["vacation"]?.ToModel<QywxApplyVacation>();


    /// <summary>
    /// 附12 假勤组件-出差/外出/加班组件（control参数为Attendance）
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    public static QywxApplyAttendance? GetAttendance(JToken? control) => control["value"]["attendance"]?.ToModel<QywxApplyAttendance>();

    /// <summary>
    /// 附13 补卡组件（control参数为PunchCorrection）
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    public static QywxApplyPunchCorrection? GetPunchCorrection(JToken? control) => control["value"]["punch_correction"]?.ToModel<QywxApplyPunchCorrection>();

    /// <summary>
    /// 附14 时长组件（control参数为DateRange）
    /// <br/>时间展示类型：halfday-日期；hour-日期+时间
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    public static QywxApplyDateRange? GetDateRange(JToken? control) => control["value"]["date_range"]?.ToModel<QywxApplyDateRange>();

    /// <summary>
    /// 附15 位置控件（control参数为Location）
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    public static QywxApplyLocation? GetLocation(JToken? control) => control["value"]["location"]?.ToModel<QywxApplyLocation>();

    /// <summary>
    /// 附16 关联审批单控件（control参数为RelatedApproval）
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    public static QywxApplyRelatedApproval? GetRelatedApproval(JToken? control) => control["value"]["related_approval"]?.FirstOrDefault()?.ToModel<QywxApplyRelatedApproval>();

    /// <summary>
    /// 附16 关联审批单控件（control参数为RelatedApproval）
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    public static List<QywxApplyRelatedApproval> GetRelatedApprovals(JToken? control) => control["value"]["related_approval"]?.ToModel<List<QywxApplyRelatedApproval>>() ?? new List<QywxApplyRelatedApproval>();

    /// <summary>
    /// 附17 公式控件（control参数为Formula）
    /// <br/>公式的值
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    public static string GetFormulaValue(JToken? control) => GetFormula(control)?.value;

    /// <summary>
    /// 附17 公式控件（control参数为Formula）
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    public static QywxApplyFormula? GetFormula(JToken? control) => control["value"]["formula"]?.ToModel<QywxApplyFormula>();

    #endregion

    /// <summary>
    /// 企业微信审批控件对象列表转JObject对象
    /// </summary>
    /// <param name="controls"></param>
    /// <returns></returns>
    public static JObject ControlsToJObject(IEnumerable<JToken?>? controls)
    {
        var json = new JObject();
        if (controls is null)
            return json;
        foreach (var item in controls)
        {
            var control = item.ToModel<QywxApplyControl?>();
            if (control != null && !string.IsNullOrWhiteSpace(control.title_zh_CN))
            {
                var value = control.GetValue();
                json[control.title_zh_CN] = value;
            }
        }

        return json;
    }

    /// <summary>
    /// 企业微信审批控件对象列表转对象
    /// </summary>
    /// <param name="controls"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T ControlsToModel<T>(IEnumerable<JToken?> controls) => (T)ControlsToModel(controls, typeof(T));

    /// <summary>
    /// 企业微信审批控件对象列表转对象
    /// </summary>
    /// <param name="controls"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static object ControlsToModel(IEnumerable<JToken?>? controls, Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        if (controls is null)
            return null;
        var model = Activator.CreateInstance(type);
        var props = new LinkedList<PropertyInfo>(type.GetProperties());
        var fields = new LinkedList<FieldInfo>(type.GetFields());
        foreach (var item in controls)
        {
            var control = item.ToModel<QywxApplyControl?>();
            if (control != null && !string.IsNullOrWhiteSpace(control.title_zh_CN))
            {
                var prop1 = props.PickValue(p => p.Value.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName == control.title_zh_CN);
                if (prop1 != null)
                {
                    var value = control.control == CONTROL_TABLE ? ControlsToModel(item["value"]["children"]?.SelectMany(child => child["list"]), prop1.PropertyType) : GetModel(item, prop1.PropertyType);
                    prop1.SetValue(model, value);
                    continue;
                }

                var field1 = fields.PickValue(p => p.Value.GetCustomAttribute<JsonPropertyAttribute>().PropertyName == control.title_zh_CN);
                if (field1 != null)
                {
                    var value = control.control == CONTROL_TABLE ? ControlsToModel(item, field1.FieldType) : GetModel(item, field1.FieldType);
                    field1.SetValue(model, value);
                }
            }
        }

        return model;
    }
}


/// <summary>
/// 企业微信审批控件, 语言文字
/// </summary>
public class QywxApplyLangText
{
    public string text { get; set; }
    public string lang { get; set; }
}

public class QywxApplySelector
{
    public string type { get; set; }
    public List<QywxApplySelectOption>? options { get; set; }
    public JArray op_relations { get; set; }

    /// <summary>
    /// 第一个选项的值
    /// </summary>
    public string Value => options?.FirstOrDefault()?.value_zh_CN;

    /// <summary>
    /// 多选值列表
    /// </summary>
    public List<string> Values => options?.Select(p => p.value_zh_CN).ToList() ?? new List<string>();
}

/// <summary>
/// 附4 日期/日期+时间控件（control参数为Date）
/// </summary>
public class QywxApplyDate
{
    private int _sTimestamp;

    /// <summary>
    /// 日期
    /// </summary>
    public const string DAY = "day";

    /// <summary>
    /// 日期+时间
    /// </summary>
    public const string HOUR = "hour";

    public string type { get; set; }

    public int s_timestamp
    {
        get => _sTimestamp;
        set
        {
            _sTimestamp = value;
            datetime = _sTimestamp > 0 ? DateTimeHelper.FromUnixTimeStamp(_sTimestamp) : null;
        }
    }

    public DateTime? datetime { get; private set; }
}

/// <summary>
/// 选项
/// </summary>
public class QywxApplySelectOption
{
    public string? key { get; set; }
    public List<QywxApplyLangText>? value { get; set; }
    public string value_zh_CN => value?.FirstOrDefault(p => p.lang == "zh_CN")?.text;
}


/// <summary>
/// 附6 成员控件（control参数为Contact，且value参数为members）
/// </summary>
public class QywxApplyMember
{
    /// <summary>
    /// 成员的userid
    /// </summary>
    [JsonProperty("userid")]
    public string id { get; set; }

    /// <summary>
    /// 成员名
    /// </summary>
    public string name { get; set; }
}

/// <summary>
/// 附7 部门控件（control参数为Contact，且value参数为departments）
/// </summary>
public class QywxApplyDepartment
{
    /// <summary>
    /// 部门id
    /// </summary>
    [JsonProperty("openapi_id")]
    public string id { get; set; }

    /// <summary>
    /// 部门名
    /// </summary>
    public string name { get; set; }
}

/// <summary>
/// 附9 附件控件（control参数为File）
/// </summary>
public class QywxApplyFile
{
    public string file_id { get; set; }
}

/// <summary>
/// 假勤组件-请假组件
/// </summary>
public class QywxApplyVacation
{
    public QywxApplySelector? selector { get; set; }

    public QywxApplyAttendance? attendance { get; set; }

    /// <summary>
    /// 请假管理中的假期类型
    /// </summary>
    public string? vacation_type_name => selector?.Value;

    /// <summary>
    /// 假勤组件时间选择范围<br/>
    /// 时间展示类型：halfday-日期；hour-日期+时间
    /// </summary>
    public QywxApplyDateRange date_range => attendance.date_range;

    /// <summary>
    /// 假勤组件类型：1-请假；2-补卡；3-出差；4-外出；5-加班
    /// </summary>
    public int type => attendance.type;
}

/// <summary>
/// 补卡信息
/// </summary>
public class QywxApplyPunchCorrection
{
    private int _time;
    public string state { get; set; }

    public int time
    {
        get => _time;
        set
        {
            _time = value;
            datetime = _time > 0 ? DateTimeHelper.FromUnixTimeStamp(_time) : null;
        }
    }

    public DateTime? datetime { get; private set; }
}

/// <summary>
/// 时长信息
/// </summary>
public class QywxApplyDateRange
{
    private int _newBegin;
    private int _newEnd;
    private int _duration;

    /// <summary>
    /// 日期
    /// </summary>
    public const string DAY = "day";

    /// <summary>
    /// (半天)日期
    /// </summary>
    public const string HALF_DAY = "halfday";

    /// <summary>
    /// 时间
    /// </summary>
    public const string HOUR = "hour";

    public string type { get; set; }

    /// <summary>
    /// 开始时间(unix时间戳)
    /// </summary>
    public int new_begin
    {
        get => _newBegin;
        set
        {
            _newBegin = value;
            begin = _newBegin > 0 ? DateTimeHelper.FromUnixTimeStamp(_newBegin) : null;
        }
    }

    /// <summary>
    /// 结束时间(unix时间戳)
    /// </summary>
    public int new_end
    {
        get => _newEnd;
        set
        {
            _newEnd = value;
            end = _newEnd > 0 ? DateTimeHelper.FromUnixTimeStamp(_newEnd) : null;
        }
    }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? begin { get; private set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? end { get; private set; }

    /// <summary>
    /// 请假时长(秒)
    /// </summary>
    [JsonProperty("new_duration")]
    public int duration
    {
        get => _duration;
        set
        {
            _duration = value;
            timespan = new TimeSpan(0, 0, _duration);
        }
    }

    public TimeSpan timespan { get; private set; }
}

/// <summary>
/// 假勤组件-出差/外出/加班信息
/// </summary>
public class QywxApplyAttendance
{
    public QywxApplyDateRange date_range { get; set; }
    public int type { get; set; }

    /// <summary>
    /// 分片信息
    /// </summary>
    public class SliceInfo
    {
        private int _duration;

        /// <summary>
        /// 系统自动计算
        /// </summary>
        public const int STATE_SYSTEM_AUTO_COMPUTED = 1;

        /// <summary>
        /// 用户修改
        /// </summary>
        public const int STATE_USER_MODIFIED = 2;

        /// <summary>
        /// 总时长，单位是秒
        /// </summary>
        public int duration
        {
            get => _duration;
            set
            {
                _duration = value;
                timespan = new TimeSpan(0, 0, _duration);
            }
        }

        public TimeSpan timespan { get; private set; }

        /// <summary>
        /// 时长计算来源类型: 1--系统自动计算;2--用户修改
        /// </summary>
        public int state { get; set; }

        /// <summary>
        /// 每一天的分片时长信息
        /// </summary>
        public List<DayItem> day_items { get; set; }
    }

    public class DayItem
    {
        private int _daytime;
        private int _duration;

        /// <summary>
        /// 日期的00:00:00时间戳，Unix时间
        /// </summary>
        public int daytime
        {
            get => _daytime;
            set
            {
                _daytime = value;
                day = _daytime > 0 ? DateTimeHelper.FromUnixTimeStamp(_daytime) : null;
            }
        }

        /// <summary>
        /// 日期
        /// </summary>
        public DateTime? day { get; private set; }

        /// <summary>
        /// 分隔当前日期的时长秒数
        /// </summary>
        public int duration
        {
            get => _duration;
            set
            {
                _duration = value;
                timespan = new TimeSpan(0, 0, _duration);
            }
        }

        public TimeSpan timespan { get; private set; }
    }
}

/// <summary>
/// 附15 位置控件（control参数为Location）
/// </summary>
public class QywxApplyLocation
{
    private int _time;

    /// <summary>
    /// 纬度，精确到6位小数
    /// </summary>
    public string latitude { get; set; }

    /// <summary>
    /// 经度，精确到6位小数
    /// </summary>
    public string longitude { get; set; }

    /// <summary>
    /// 地点标题
    /// </summary>
    public string title { get; set; }

    /// <summary>
    /// 地点详情地址
    /// </summary>
    public string address { get; set; }

    /// <summary>
    /// 选择地点的时间(时间戳)
    /// </summary>
    public int time
    {
        get => _time;
        set
        {
            _time = value;
            address_time = _time > 0 ? DateTimeHelper.FromUnixTimeStamp(_time) : null;
        }
    }

    /// <summary>
    /// 选择地点的时间
    /// </summary>
    public DateTime? address_time { get; private set; }
}

/// <summary>
/// 关联审批单
/// </summary>
public class QywxApplyRelatedApproval
{
    /// <summary>
    /// 关联审批单的模板名
    /// </summary>
    public List<QywxApplyLangText>? template_names { get; set; }

    private int _createTime;

    /// <summary>
    /// 关联审批单的模板名
    /// </summary>
    public string? template_name => template_names?.FirstOrDefault(p => p.lang == "zh_CN")?.text;

    /// <summary>
    /// 关联审批单的审批单号
    /// </summary>
    public string sp_no { get; set; }

    /// <summary>
    /// 关联审批单的状态
    /// </summary>
    public int sp_status { get; set; }

    /// <summary>
    /// 关联审批单的提单时间
    /// </summary>
    public int create_time
    {
        get => _createTime;
        set
        {
            _createTime = value;
            add_time = _createTime > 0 ? DateTimeHelper.FromUnixTimeStamp(_createTime) : null;
        }
    }

    /// <summary>
    /// 关联审批单的提单时间
    /// </summary>
    public DateTime? add_time { get; private set; }

    /// <summary>
    /// 关联审批单的提单者
    /// </summary>
    public string name { get; set; }
}

/// <summary>
/// 公式
/// </summary>
public class QywxApplyFormula
{
    public string value { get; set; }
}

/// <summary>
/// 企业微信审批控件
/// </summary>
public class QywxApplyControl
{
    /// <summary>
    /// 控件类型
    /// </summary>
    public string control { get; set; }

    public string id { get; set; }
    public List<QywxApplyLangText>? title { get; set; }

    /// <summary>
    /// 获取中文标题
    /// </summary>
    public string title_zh_CN => title?.FirstOrDefault(p => p.lang == "zh_CN")?.text;

    public JObject value { get; set; }

    public JToken GetValue()
    {
        switch (control)
        {
            case CONTROL_TEXT:
            case CONTROL_TEXTAREA:
                return value["text"]; // string
            case CONTROL_NUMBER:
                return value["new_number"]; // string
            case CONTROL_MONEY:
                return value["new_money"]; // string
            case CONTROL_DATE:
                return value["date"]; // QywxApplyDate
            case CONTROL_SELECTOR:
                return value["selector"]; // QywxApplySelector
            case CONTROL_CONTACT:
                // List<QywxApplyMember|QywxApplyDepartment>
                return (value["members"] as JArray)?.Count > 0 ? value["members"] : value["departments"];
            case CONTROL_FILE:
                return value["files"]; // List<QywxApplyFile>
            case CONTROL_TABLE:
                // 2023-7-28 Ciaran 表格直接转对象JObject
                return ControlsToJObject(value["children"]?.SelectMany(child1 => child1["list"]));
            case CONTROL_VACATION:
                return value["vacation"]; // QywxApplyVacation
            case CONTROL_ATTENDANCE:
                return value["attendance"]; // QywxApplyAttendance
            case CONTROL_PUNCH_CORRECTION:
                return value["punch_correction"]; // QywxApplyPunchCorrection
            case CONTROL_DATE_RANGE:
                return value["date_range"]; // QywxApplyDateRange
            case CONTROL_LOCATION:
                return value["location"]; // QywxApplyLocation
            case CONTROL_RELATED_APPROVAL:
                return value["related_approval"]; // List<QywxApplyRelatedApproval>
            case CONTROL_FORMULA:
                return value["formula"]; // QywxApplyFormula
        }

        return null;
    }
}


/// <summary>
/// 企业微信审批, 开票公司
/// </summary>
public class QywxApplyTestJObject
{
    [JsonProperty("文本控件")] public string text1 { get; set; }
    [JsonProperty("多行文本控件")] public string textarea1 { get; set; }
    [JsonProperty("数字控件")] public string num1 { get; set; }
    [JsonProperty("金额控件")] public string money1 { get; set; }
    [JsonProperty("日期/日期+时间控件")] public QywxApplyDate date1 { get; set; }
    [JsonProperty("单选/多选控件")] public QywxApplySelector select1 { get; set; }
    [JsonProperty("成员控件")] public List<QywxApplyMember> members1 { get; set; }
    [JsonProperty("部门控件")] public List<QywxApplyDepartment> departments1 { get; set; }
    [JsonProperty("附件控件")] public List<QywxApplyFile> files1 { get; set; }
    [JsonProperty("明细控件")] public QywxApplyTestTableJObject table1 { get; set; }
    [JsonProperty("假勤组件-请假组件")] public QywxApplyVacation vacation1 { get; set; }
    [JsonProperty("假勤组件-出差/外出/加班组件")] public QywxApplyAttendance attendance1 { get; set; }
    [JsonProperty("补卡组件")] public QywxApplyPunchCorrection punch_correction1 { get; set; }
    [JsonProperty("时长组件")] public QywxApplyDateRange date_range1 { get; set; }
    [JsonProperty("位置控件")] public QywxApplyLocation location1 { get; set; }
    [JsonProperty("关联审批单控件")] public List<QywxApplyRelatedApproval> related_approvals1 { get; set; }
    [JsonProperty("公式控件")] public QywxApplyFormula formula1 { get; set; }

    public class QywxApplyTestTableJObject
    {
        [JsonProperty("明细内文本控件")] public string text1 { get; set; }
        [JsonProperty("明细内日期/日期+时间控件")] public QywxApplyDate date1 { get; set; }
        [JsonProperty("明细内假勤组件-请假组件")] public QywxApplyVacation vacation1 { get; set; }
    }
}


/// <summary>
/// 企业微信审批, 开票公司
/// </summary>
public class QywxApplyTestModel
{
    [JsonProperty("文本控件")] public string text1 { get; set; }
    [JsonProperty("多行文本控件")] public string textarea1 { get; set; }
    [JsonProperty("数字控件")] public double num1 { get; set; }
    [JsonProperty("金额控件")] public decimal money1 { get; set; }
    [JsonProperty("日期/日期+时间控件")] public DateTime? date1 { get; set; }
    [JsonProperty("单选/多选控件")] public List<string> select1 { get; set; }
    [JsonProperty("成员控件")] public List<QywxApplyMember> members1 { get; set; }
    [JsonProperty("部门控件")] public List<QywxApplyDepartment> departments1 { get; set; }
    [JsonProperty("附件控件")] public List<QywxApplyFile> files1 { get; set; }
    [JsonProperty("明细控件")] public QywxApplyTestTableModel table1 { get; set; }
    [JsonProperty("假勤组件-请假组件")] public QywxApplyVacation vacation1 { get; set; }
    [JsonProperty("假勤组件-出差/外出/加班组件")] public QywxApplyAttendance attendance1 { get; set; }
    [JsonProperty("补卡组件")] public QywxApplyPunchCorrection punch_correction1 { get; set; }
    [JsonProperty("时长组件")] public QywxApplyDateRange date_range1 { get; set; }
    [JsonProperty("位置控件")] public QywxApplyLocation location1 { get; set; }
    [JsonProperty("关联审批单控件")] public List<QywxApplyRelatedApproval> related_approvals1 { get; set; }
    [JsonProperty("公式控件")] public decimal? formula1 { get; set; }

    public class QywxApplyTestTableModel
    {
        [JsonProperty("明细内文本控件")] public string text1 { get; set; }
        [JsonProperty("明细内日期/日期+时间控件")] public DateTime? date1 { get; set; }
        [JsonProperty("明细内假勤组件-请假组件")] public QywxApplyVacation vacation1 { get; set; }
    }
}

public class QywxApplyTest
{
    /// <summary>
    /// 企业微信 审批读取控件值
    /// </summary>
    public static void TestQywxApplyGetControlValue()
    {
        string jsonstr = JsonStr;

        var json = jsonstr.FromJson();
        if (json is null)
        {
            return;
        }

        var errcode = json["errcode"]?.ToModel<long>();
        var errmsg = json["errmsg"]?.ToModel<string>();
        if (errcode != 0)
        {
            // Logger.Log4Net.AppLog.Error("获取审批失败, msg=" + errmsg);
            return;
        }

        var contents = json["info"]?["apply_data"]?["contents"] as JArray;
        if (contents == null) return;

        // 映射标题和控件
        var controls = contents.ToDictionary(item => (item["title"] as JArray)?.FirstOrDefault(value1 => value1["lang"]?.ToModel<string>() == "zh_CN")?["text"]?.ToModel<string>(), item => item);

        #region 写法1

        string GetValue(string? name) => QywxApplyHelper.GetValue(controls.Get(name));
        List<string> GetValues(string? name) => QywxApplyHelper.GetValues(controls.Get(name));

        T GetModel<T>(string? name) => QywxApplyHelper.GetModel<T>(controls.Get(name));

        var text1 = GetValue("文本控件");
        var textarea1 = GetValue("多行文本控件");
        var num1 = GetValue("数字控件");
        var num2 = GetModel<double>("数字控件");
        var money1 = GetValue("金额控件");
        var money2 = GetModel<decimal>("金额控件");
        var money3 = GetModel<decimal?>("金额控件");
        var date1 = GetValue("日期/日期+时间控件");
        var date2 = GetModel<DateTime>("日期/日期+时间控件");
        var date3 = GetModel<DateTime?>("日期/日期+时间控件");
        var select1 = GetValue("单选/多选控件");
        var selects = GetValues("单选/多选控件");
        var selects2 = GetModel<QywxApplySelector>("单选/多选控件");
        var selects3 = GetModel<string[]>("单选/多选控件");

        var member1 = GetModel<QywxApplyMember>("成员控件");
        var members = GetModel<List<QywxApplyMember>>("成员控件");

        var department1 = GetModel<QywxApplyDepartment>("部门控件");
        var departments = GetModel<List<QywxApplyDepartment>>("部门控件");

        var file1 = GetValue("附件控件");
        var files = GetValues("附件控件");
        var file2 = GetModel<QywxApplyFile>("附件控件");
        var files2 = GetModel<List<QywxApplyFile>>("附件控件");

        var table_items = GetTableControls(controls.Get("明细控件"));
        var table_text1 = QywxApplyHelper.GetValue(table_items.Get("明细内文本控件"));
        var table_date1 = QywxApplyHelper.GetModel<QywxApplyDate>(table_items.Get("明细内日期/日期+时间控件"));
        var table_vacation1 = QywxApplyHelper.GetModel<QywxApplyVacation>(table_items.Get("明细内假勤组件-请假组件"));

        var vacation = GetModel<QywxApplyVacation>("假勤组件-请假组件");
        var attendance = GetModel<QywxApplyAttendance>("假勤组件-出差/外出/加班组件");
        var punch_correction = GetModel<QywxApplyPunchCorrection>("补卡组件");

        var date_range = GetModel<QywxApplyDateRange>("时长组件");
        var location = GetModel<QywxApplyLocation>("位置控件");

        var related_approval = GetModel<QywxApplyRelatedApproval>("关联审批单控件");
        var related_approvals = GetModel<List<QywxApplyRelatedApproval>>("关联审批单控件");

        var formula = GetValue("公式控件");
        var formula2 = GetModel<string>("公式控件");
        var formula3 = GetModel<QywxApplyFormula>("公式控件");
        var formula4 = GetModel<double>("公式控件");
        #endregion

        #region 写法2

        var jmodel = ControlsToJObject(contents);
        var model = jmodel.ToModel<QywxApplyTestJObject>();

        #endregion

        #region 写法3

        var model2 = ControlsToModel<QywxApplyTestModel>(contents);

        #endregion

        int i = 1;
    }


    #region jsonstr

    public const string JsonStr = @"
{
  ""errcode"": 0,
  ""errmsg"": ""ok"",
  ""info"": {
    ""sp_no"": ""201909270002"",
    ""sp_name"": ""全字段"",
    ""sp_status"": 1,
    ""template_id"": ""Bs5KJ2NT4ncf4ZygaE8MB3779yUW8nsMaJd3mmE9v"",
    ""apply_time"": 1569584428,
    ""applyer"": {
      ""userid"": ""WuJunJie"",
      ""partyid"": ""2""
    },
    ""sp_record"": [
      {
        ""sp_status"": 1,
        ""approverattr"": 1,
        ""details"": [
          {
            ""approver"": {
              ""userid"": ""WuJunJie""
            },
            ""speech"": """",
            ""sp_status"": 1,
            ""sptime"": 0,
            ""media_id"": []
          },
          {
            ""approver"": {
              ""userid"": ""WangXiaoMing""
            },
            ""speech"": """",
            ""sp_status"": 1,
            ""sptime"": 0,
            ""media_id"": []
          }
        ]
      }
    ],
    ""notifyer"": [
      {
        ""userid"": ""LiuXiaoGang""
      }
    ],
    ""apply_data"": {
      ""contents"": [
        {
          ""control"": ""Text"",
          ""title"": [
            {
              ""text"": ""文本控件"",
              ""lang"": ""zh_CN""
            }
          ],
          ""value"": {
            ""text"": ""文本填写的内容""
          }
        },
        {
          ""control"": ""Textarea"",
          ""title"": [
            {
              ""text"": ""多行文本控件"",
              ""lang"": ""zh_CN""
            }
          ],
          ""value"": {
            ""text"": ""多行文本填写的内容""
          }
        },
        {
          ""control"": ""Number"",
          ""title"": [
            {
              ""text"": ""数字控件"",
              ""lang"": ""zh_CN""
            }
          ],
          ""value"":{
            ""new_number"": ""700""
          }
        },
        {
          ""control"": ""Money"",
          ""title"": [
            {
              ""text"": ""金额控件"",
              ""lang"": ""zh_CN""
            }
          ],
          ""value"": {
            ""new_money"": ""700""
          }
        },

        {
          ""control"": ""Date"",
          ""title"": [
            {
              ""text"": ""日期/日期+时间控件"",
              ""lang"": ""zh_CN""
            }
          ],
          ""value"": {
            ""date"": {
              ""type"": ""day"",
              ""s_timestamp"": ""1569859200""
            }
          }
        },
        {
          ""control"": ""Selector"",
          ""title"": [
            {
              ""text"": ""单选/多选控件"",
              ""lang"": ""zh_CN""
            }
          ],
          ""value"": {
            ""selector"": {
              ""type"": ""multi"",
              ""options"": [
                {
                  ""key"": ""option-15111111111"",
                  ""value"": [
                    {
                      ""text"": ""选项1"",
                      ""lang"": ""zh_CN""
                    }
                  ]
                },
                {
                  ""key"": ""option-15222222222"",
                  ""value"": [
                    {
                      ""text"": ""选项2"",
                      ""lang"": ""zh_CN""
                    }
                  ]
                }
              ]
            }
          }
        },
        {
          ""control"": ""Contact"",
          ""title"": [
            {
              ""text"": ""成员控件"",
              ""lang"": ""zh_CN""
            }
          ],
          ""value"": {
            ""members"": [
              {
                ""userid"": ""WuJunJie"",
                ""name"": ""Jackie""
              },
              {
                ""userid"": ""WangXiaoMing"",
                ""name"": ""Tom""
              }
            ]
          }
        },
        {
          ""control"": ""Contact"",
          ""title"": [
            {
              ""text"": ""部门控件"",
              ""lang"": ""zh_CN""
            }
          ],
          ""value"": {
            ""departments"": [
              {
                ""openapi_id"": ""2"",
                ""name"": ""销售部""
              },
              {
                ""openapi_id"": ""3"",
                ""name"": ""生产部""
              }
            ]
          }
        },

        {
          ""control"": ""File"",
          ""title"": [
            {
              ""text"": ""附件控件"",
              ""lang"": ""zh_CN""
            }
          ],
          ""value"": {
            ""files"": [
              {
                ""file_id"": ""WWCISP_v2z8qZENw2qwSiNroVKykbxxMXvmI1lELzG-fo25Y9n1duozezKEu6zSIvOHPCd9_8s934AJncRz5f9G4E_nCQonUHLdiAnCLjfZQQwVaiG7krKzyGB1MpYa9ZVkk0gQ7P8HvO_SOdwzLwpyUZ3Tm2ApyoO_78nTM-iEkf_TILqXuYxKd7ByYL34wMA9Czf6Iy151tHbcYNvbNZZHTnL4UMQdohJ_MPYA2Wz00IebZb3_UuIk5MdJSH_IKlZn9Ms5""
              },
              {
                ""file_id"": ""WWCISP_gZ3BMg5hwI1Adi16NwzJgpi9zp6QQjMdYcuemVWBeHnmMK3QJOYiIIkHvRIh0ysZcAo6gJp069o5tx7qxVzin1Q9LKswff624E1qCCmt088ISBVPScoqEiG4YTI_Kltrqn7b0wvMTudd9lIE3ywgHatPRWKxsHNsSxEY_FuaFWlGHzxcYKNq_LIfVBXZGji-C5bXp23MwpTcCXYfWPfSEpEeXW5c5sQscY_MeW5uc0gITpeFKFXARXmKC62_u7Ln""
              }
            ]
          }
        },
        {
          ""control"": ""Table"",
          ""title"": [
            {
              ""text"": ""明细控件"",
              ""lang"": ""zh_CN""
            }
          ],
          ""value"": {
            ""children"": [
              {
                ""list"": [
                  {
                    ""control"": ""Text"",
                    ""id"": ""Text-15111111111"",
                    ""title"": [
                      {
                        ""text"": ""明细内文本控件"",
                        ""lang"": ""zh_CN""
                      }
                    ],
                    ""value"": {
                      ""text"": ""明细文本""
                    }
                  },
                  {
                    ""control"": ""Date"",
                    ""title"": [
                      {
                        ""text"": ""明细内日期/日期+时间控件"",
                        ""lang"": ""zh_CN""
                      }
                    ],
                    ""value"": {
                      ""date"": {
                        ""type"": ""day"",
                        ""s_timestamp"": ""1569859200""
                      }
                    }
                  },
                  {
                    ""control"": ""Vacation"",
                    ""title"": [
                      {
                        ""text"": ""明细内假勤组件-请假组件"",
                        ""lang"": ""zh_CN""
                      }
                    ],
                    ""value"": {
                      ""vacation"": {
                        ""selector"": {
                          ""type"": ""single"",
                          ""options"": [
                            {
                              ""key"": ""3"",
                              ""value"": [
                                {
                                  ""text"": ""病假"",
                                  ""lang"": ""zh_CN""
                                }
                              ]
                            }
                          ],
                          ""exp_type"": 0
                        },
                        ""attendance"": {
                          ""date_range"": {
                            ""type"": ""hour"",
                            ""new_begin"": 1568077200,
                            ""new_end"": 1568368800,
                            ""new_duration"": 291600
                          },
                          ""type"": 1
                        }
                      }
                    }
                  },
                ]
              }
            ]
          }
        },
        {
          ""control"": ""Vacation"",
          ""title"": [
            {
              ""text"": ""假勤组件-请假组件"",
              ""lang"": ""zh_CN""
            }
          ],
          ""value"": {
            ""vacation"": {
              ""selector"": {
                ""type"": ""single"",
                ""options"": [
                  {
                    ""key"": ""3"",
                    ""value"": [
                      {
                        ""text"": ""病假"",
                        ""lang"": ""zh_CN""
                      }
                    ]
                  }
                ],
                ""exp_type"": 0
              },
              ""attendance"": {
                ""date_range"": {
                  ""type"": ""hour"",
                  ""new_begin"": 1568077200,
                  ""new_end"": 1568368800,
                  ""new_duration"": 291600
                },
                ""type"": 1
              }
            }
          }
        },
        {
          ""control"": ""Attendance"",
          ""title"": [
            {
              ""text"": ""假勤组件-出差/外出/加班组件"",
              ""lang"": ""zh_CN""
            }
          ],
          ""value"": {
            ""attendance"": {
              ""date_range"": {
                ""type"": ""halfday"",
                ""new_begin"": 1599494400,
                ""new_end"": 1599667199,
                ""new_duration"": 172800
              },
              ""type"": 4,
              ""slice_info"":{
                ""day_items"" :[
                  {
                    ""daytime"":1599494400,
                    ""duration"":86400
                  },
                  {
                    ""daytime"":1599580800,
                    ""duration"":86400
                  }
                ],
                ""duration"":172800,
                ""state"":1

              }
            }
          }
        },

        {
          ""control"": ""PunchCorrection"",
          ""title"": [
            {
              ""text"": ""补卡组件"",
              ""lang"": ""zh_CN""
            }
          ],
          ""value"": {
            ""punch_correction"": {
              ""state"":""迟到"",
              ""time"":1570550400
            }
          }
        },
        {
          ""control"": ""DateRange"",
          ""title"": [
            {
              ""text"": ""时长组件"",
              ""lang"": ""zh_CN""
            }
          ],
          ""value"": {
            ""date_range"": {
              ""type"": ""halfday"",
              ""new_begin"": 1570550400,
              ""new_end"": 1570593600,
              ""new_duration"": 86400
            }
          }
        },
        {
          ""control"": ""Location"",
          ""title"": [
            {
              ""text"": ""位置控件"",
              ""lang"": ""zh_CN""
            }
          ],
          ""value"": {
            ""location"": {
              ""latitude"": ""30.547239"",
              ""longitude"": ""104.063291"",
              ""title"": ""腾讯科技(成都)有限公司(腾讯成都大厦)"",
              ""address"": ""四川省成都市武侯区天府三街198号腾讯成都大厦A座"",
              ""time"": 1605690460
            }
          }
        },
        {
          ""control"": ""RelatedApproval"",
          ""title"": [
            {
              ""text"": ""关联审批单控件"",
              ""lang"": ""zh_CN""
            }
          ],
          ""value"": {
            ""related_approval"": [
              {
                ""template_names"": [{
                  ""text"": ""模板A"",
                  ""lang"": ""zh_CN""
                },
                  {
                    ""text"": """",
                    ""lang"": ""en""
                  }],
                ""sp_status"": 1,
                ""name"": ""小明"",
                ""create_time"": 1605690437,
                ""sp_no"": ""202011180001""
              }
            ]
          }
        },

        {
          ""control"": ""Formula"",
          ""title"": [
            {
              ""text"": ""公式控件"",
              ""lang"": ""zh_CN""
            }
          ],
          ""value"": {
            ""formula"": {
              ""value"": ""5.0""
            }
          }
        }
      ]
    },
    ""comments"": [
      {
        ""commentUserInfo"": {
          ""userid"": ""WuJunJie""
        },
        ""commenttime"": 1569584111,
        ""commentcontent"": ""这是备注信息"",
        ""commentid"": ""6741314136717778040"",
        ""media_id"": [
          ""WWCISP_Xa1dXIyC9VC2vGTXyBjUXh4GQ31G-a7jilEjFjkYBfncSJv0kM1cZAIXULWbbtosVqA7hprZIUkl4GP0DYZKDrIay9vCzeQelmmHiczwfn80v51EtuNouzBhUBTWo9oQIIzsSftjaVmd4EC_dj5-rayfDl6yIIRdoUs1V_Gz6Pi3yH37ELOgLNAPYUSJpA6V190Xunl7b0s5K5XC9c7eX5vlJek38rB_a2K-kMFMiM1mHDqnltoPa_NT9QynXuHi""
        ]
      }
    ]
  }
}

";

    #endregion
}