using System;

namespace FPL.OutPut
{
    public class Debugger
    {
        public static readonly string[] Contents =
        {
            "应输入 {0}",
            "应输入类型或文件尾",
            "应输入类型",
            "意外的字符 {0}",
            "{0} 无效",
            "应输入标识符",
            "当前上下文中已经存在 {0} 的定义",
            "当前上下文不存在 {0} 的定义",
            "类型 {0} 中已存在 {1} 的定义",
            "类型 {0} 中不存在 {1} 的定义",
            "整数超出范围",
            "浮点数超出范围",
            "表达式错误",
            "语法错误",
            "语句错误",
            "语句错误或大括号不匹配",
            "暂无重写",
            "暂无重载",
            "运算符 {0} 无法用于 {1} 类型的操作数",
            "运算符 {0} 无法用于类型 {1} 和 {2} 的操作数",
            "无法将类型 {0} 转换为类型 {1}",
            "缺少参数",
            "{0} 方法没有采用 {1} 个参数的重载",
            "未找到程序入口点",
            "对象引用对于非静态的字段、方法或属性 {0} 是必须的",
            "不是所有路径都有返回值",
            "不允许有参数",
            "该类型不存在构造函数",
            "该类型不存在 {0} 个参数的构造函数",
            "没有要中断或继续的循环",
            "只有赋值，函数调用和new 对象表达式可用作语句",
            "需要一个类型可转换为 {0} 的对象",
            "类型 {0} 已定义了一个名为 {1} 的具有相同参数类型的成员",
            "类型 {0} 中没有名为 {1} 的符合的函数重载",

        };

        public static void LogError(string s, LogContent content, params object[] parm)
        {
            if (parm.Length == 0)
            {
                Console.WriteLine("错误：" + s + Contents[(int)content]);
            }
            else
            {
                Console.WriteLine("错误：" + s + Contents[(int)content], parm);
            }
        }
        public static void LogWarning(string s, LogContent content, params object[] parm)
        {
            if (parm.Length == 0)
            {
                Console.WriteLine("警告：" + s + Contents[(int)content]);
            }
            else
            {
                Console.WriteLine("警告：" + s + Contents[(int)content], parm);
            }
        }
        public static void Log(string s, LogContent content, params object[] parm)
        {
            if (parm.Length == 0)
            {
                Console.WriteLine("信息：" + s + Contents[(int)content]);
            }
            else
            {
                Console.WriteLine("信息：" + s + Contents[(int)content], parm);
            }
        }
    }

    public enum LogContent
    {
        /// <summary>
        /// 应输入 {0}
        /// </summary>
        SthExpect,

        /// <summary>
        /// 应输入类型或文件尾
        /// </summary>
        InPutTypeOrFileEnd,

        /// <summary>
        /// 应输入类型
        /// </summary>
        InPutType,

        /// <summary>
        /// 意外的字符 {0}
        /// </summary>
        SthUnexpect,

        /// <summary>
        /// {0} 无效
        /// </summary>
        SthUseless,

        /// <summary>
        /// 应输入标识符
        /// </summary>
        IDExpect,

        /// <summary>
        /// 当前上下文中已存在 {0} 的定义
        /// </summary>
        ExistingDefinition,

        /// <summary>
        /// 当前上下文不存在 {0} 的定义
        /// </summary>
        NotExistingDefinition,

        /// <summary>
        /// 类型 {0} 中已存在 {1} 的定义
        /// </summary>
        ExistingDefinitionInType,

        /// <summary>
        /// 类型 {0} 中不存在 {1} 的定义
        /// </summary>
        NotExistingDefinitionInType,

        /// <summary>
        /// 整数超出范围
        /// </summary>
        NumberOutOfRange,

        /// <summary>
        /// 浮点数超出范围
        /// </summary>
        FloatOutOfRange,

        /// <summary>
        /// 表达式错误
        /// </summary>
        ExprError,

        /// <summary>
        /// 语法错误
        /// </summary>
        GarmmarError,

        /// <summary>
        /// 语句错误
        /// </summary>
        SentenceError,
        /// <summary>
        /// 语句错误或大括号不匹配
        /// </summary>
        SentenceErrorOrRBraceDoesNotMatch,

        /// <summary>
        /// 暂无重写
        /// </summary>
        NoOverride,

        /// <summary>
        /// 暂无重载
        /// </summary>
        NoOverload,

        /// <summary>
        /// 运算符 {0} 无法用于 {1} 类型的操作数
        /// </summary>
        OperandNonsupport,

        /// <summary>
        /// 运算符 {0} 无法用于类型 {1} 和 {2} 的操作数
        /// </summary>
        OperandNonsupportD,

        /// <summary>
        /// 无法将类型 {0} 转换为类型 {1}
        /// </summary>
        UnableToConvertType,

        /// <summary>
        /// 缺少参数
        /// </summary>
        MissingParam,

        /// <summary>
        /// {0} 方法没有采用 {1} 个参数的重载
        /// </summary>
        NumberOfParamDoesNotMatch,

        /// <summary>
        /// 未找到程序入口点
        /// </summary>
        NoEntranceMethod,

        /// <summary>
        /// 对象引用对于非静态的字段、方法或属性 {0} 是必须的
        /// </summary>
        ShouldBeingInstanced,

        /// <summary>
        /// 不是所有路径都有返回值
        /// </summary>
        NotAllPathHaveReturnValue,

        /// <summary>
        /// 不允许有参数
        /// </summary>
        HaveParmUnallowed,

        /// <summary>
        /// 该类型不存在构造函数
        /// </summary>
        HaventConstructor,

        /// <summary>
        /// 该类型不存在 {0} 个参数的构造函数
        /// </summary>
        ConstructorParmDoesNotMatch,

        /// <summary>
        /// 没有要中断或继续的循环
        /// </summary>
        NoLoopToBreakOrContinue,

        /// <summary>
        /// 只有赋值，函数调用和new 对象表达式可用作语句
        /// </summary>
        NotSentence,

        /// <summary>
        /// 需要一个类型可转换为 {0} 的对象
        /// </summary>
        ReturnValueMissing,

        ///类型 {0} 已定义了一个名为 {1} 的具有相同参数类型的成员
        ExistingDefinitionInTypeWithParm,

        /// <summary>
        /// 类型 {0} 中没有名为 {1} 的符合的函数重载
        /// </summary>
        NotExistingMatchOverload,
    }
}
