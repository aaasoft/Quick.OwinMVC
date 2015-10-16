using System;
using System.Collections.Generic;
using System.Text;

namespace SvnManage.Utils
{
    public class UnitStringConverting
    {
        /// <summary>
        /// 单位字符串列表
        /// </summary>
        List<String> UnitList;
        /// <summary>
        /// 单位之间的换算比例列表
        /// </summary>
        List<Decimal> UnitsScaleList;

        /// <summary>
        /// 时间单位字符串转换
        /// </summary>
        public static UnitStringConverting TimeUnitStringConverting
        {
            get
            {
                String[] TimeUnitArray = new String[] 
                {
                    "毫秒", "秒", "分钟", "小时", "天"
                };
                Decimal[] TimeUnitConversionArray = new Decimal[]
                {
                    1000, 60, 60, 24
                };
                return new UnitStringConverting(TimeUnitArray, TimeUnitConversionArray);
            }
        }

        /// <summary>
        /// 存储单位字符串转换
        /// </summary>
        public static UnitStringConverting StorageUnitStringConverting
        {
            get
            {
                String[] StorageUnitArray = new String[]
                {
                    "", "K", "M", "G", "T", "P", "E", "Z", "Y" 
                };
                Decimal StorageUnitConvert = 1024;
                return new UnitStringConverting(StorageUnitArray, StorageUnitConvert);
            }
        }

        public UnitStringConverting(String[] UnitArray, Decimal UnitConversion)
        {
            Decimal[] UnitConversionArray = null;
            if (UnitArray != null && UnitArray.Length >= 2)
            {
                UnitConversionArray = new Decimal[UnitArray.Length - 1];
                for (int i = 0; i <= UnitConversionArray.Length - 1; i++)
                {
                    UnitConversionArray[i] = UnitConversion;
                }
            }
            Init(UnitArray, UnitConversionArray);
        }

        public UnitStringConverting(String[] UnitArray, Decimal[] UnitsScaleArray)
        {
            Init(UnitArray, UnitsScaleArray);
        }

        private void Init(String[] UnitArray, Decimal[] UnitsScaleArray)
        {
            if (UnitArray == null || UnitArray.Length == 0 || UnitsScaleArray == null || UnitArray.Length != UnitsScaleArray.Length + 1)
                throw new FormatException("转换单位数组参数不正确");

            this.UnitList = new List<string>(UnitArray);
            this.UnitsScaleList = new List<decimal>(UnitsScaleArray);
        }

        #region 得到合适单位
        /// <summary>
        /// 得到指定最小单位数量的合适单位序号
        /// </summary>
        /// <param name="MinUnitNumber">最小单位数量</param>
        /// <returns></returns>
        public Int32 GetFitUnitIndex(Decimal MinUnitNumber)
        {
            var TmpNumber = MinUnitNumber;
            for (int i = 0; i <= UnitsScaleList.Count - 1; i++)
            {
                var tmpUnitConversion = UnitsScaleList[i];
                TmpNumber = TmpNumber / tmpUnitConversion;
                if (Math.Abs(TmpNumber) < 1)
                {
                    return i;
                }
            }
            return UnitList.Count - 1;
        }

        /// <summary>
        /// 得到指定最小单位数量的合适单位字符串
        /// </summary>
        /// <param name="MinUnitNumber">最小单位数量</param>
        /// <returns></returns>
        public String GetFitUnitString(Decimal MinUnitNumber)
        {
            return UnitList[GetFitUnitIndex(MinUnitNumber)];
        }
        #endregion

        #region 得到指定单位和数量的最小单位数量

        /// <summary>
        /// 得到指定单位和数量的最小单位数量
        /// </summary>
        /// <param name="UnitNumber">数量</param>
        /// <param name="UnitIndex">单位序号</param>
        /// <returns></returns>
        public Decimal GetMinUnitUnits(Decimal UnitNumber, Int32 UnitIndex)
        {
            var TmpValue = UnitNumber;
            for (int i = UnitIndex - 1; i >= 0; i--)
            {
                TmpValue = TmpValue * UnitsScaleList[i];
            }
            return TmpValue;
        }

        /// <summary>
        /// 得到指定单位和数量的最小单位数量
        /// </summary>
        /// <param name="UnitNumber">数量</param>
        /// <param name="Unit">单位字符串</param>
        /// <returns></returns>
        public Decimal GetMinUnitUnits(Decimal UnitNumber, String Unit)
        {
            var UnitIndex = UnitList.IndexOf(Unit);
            if (UnitIndex < 0)
                throw new FormatException("在单位列表中未找到指定的单位。");
            return GetMinUnitUnits(UnitNumber, UnitIndex);
        }

        #endregion

        #region 得到指定最小单位数量在指定单位的值
        /// <summary>
        /// 得到指定最小单位数量在指定单位的值
        /// </summary>
        /// <param name="MinUnitNumber">最小单位数量</param>
        /// <param name="UnitIndex">单位序号</param>
        /// <returns></returns>
        public Decimal GetUnits(Decimal MinUnitNumber, Int32 UnitIndex)
        {
            var tmpValue = MinUnitNumber;
            for (int i = 0; i <= UnitIndex - 1; i++)
            {
                tmpValue = tmpValue / UnitsScaleList[i];
            }
            return tmpValue;
        }

        /// <summary>
        /// 得到指定最小单位数量在指定单位的值
        /// </summary>
        /// <param name="MinUnitNumber">最小单位数量</param>
        /// <param name="Unit">单位字符串</param>
        /// <returns></returns>
        public Decimal GetUnits(Decimal MinUnitNumber, String Unit)
        {
            var UnitIndex = UnitList.IndexOf(Unit);
            if (UnitIndex < 0)
                throw new FormatException("在单位列表中未找到指定的单位。");
            return GetUnits(MinUnitNumber, UnitIndex);
        }
        #endregion

        #region 得到合适单位与指定小数点位数的最小单位数量转换为合适单位后的字符串

        /// <summary>
        /// 得到合适单位与指定小数点位数的最小单位数量转换为合适单位后的字符串
        /// </summary>
        /// <param name="MinUnitNumber">最小单位数量</param>
        /// <returns></returns>
        public String GetString(Decimal MinUnitNumber)
        {
            return GetString(MinUnitNumber, 0, false);
        }

        /// <summary>
        /// 得到合适单位与指定小数点位数的最小单位数量转换为合适单位后的字符串
        /// </summary>
        /// <param name="MinUnitNumber">最小单位数量</param>
        /// <param name="NumberOfDecimalPlaces">小数点位数</param>
        /// <param name="SpaceBetweenNumberAndUnit">数字与单位之间是否加空格</param>
        /// <returns></returns>
        public String GetString(Decimal MinUnitNumber, Int32 NumberOfDecimalPlaces, Boolean SpaceBetweenNumberAndUnit)
        {
            var UnitIndex = GetFitUnitIndex(MinUnitNumber);
            return GetString(MinUnitNumber, UnitIndex, NumberOfDecimalPlaces, SpaceBetweenNumberAndUnit);
        }

        /// <summary>
        /// 得到指定单位与小数点位数的最小单位数量转换为指定单位后的字符串
        /// </summary>
        /// <param name="MinUnitNumber">最小单位数量</param>
        /// <param name="Unit">单位字符串</param>
        /// <param name="NumberOfDecimalPlaces">小数点位数</param>
        /// <param name="SpaceBetweenNumberAndUnit">数字与单位之间是否加空格</param>
        /// <returns></returns>
        public String GetString(Decimal MinUnitNumber, String Unit, Int32 NumberOfDecimalPlaces, Boolean SpaceBetweenNumberAndUnit)
        {
            var UnitIndex = UnitList.IndexOf(Unit);
            if (UnitIndex < 0)
                throw new FormatException("在单位列表中未找到指定的单位。");
            return GetString(MinUnitNumber, UnitIndex, NumberOfDecimalPlaces, SpaceBetweenNumberAndUnit);
        }

        /// <summary>
        /// 得到指定单位与小数点位数的最小单位数量转换为指定单位后的字符串
        /// </summary>
        /// <param name="MinUnitNumber">最小单位数量</param>
        /// <param name="UnitIndex">单位序号</param>
        /// <param name="NumberOfDecimalPlaces">小数点位数</param>
        /// <param name="SpaceBetweenNumberAndUnit">数字与单位之间是否加空格</param>
        /// <returns></returns>
        public String GetString(Decimal MinUnitNumber, Int32 UnitIndex, Int32 NumberOfDecimalPlaces, Boolean SpaceBetweenNumberAndUnit)
        {
            StringBuilder sb = new StringBuilder();

            var unitUnits = GetUnits(MinUnitNumber, UnitIndex);
            var unitUnitsString = unitUnits.ToString("F" + NumberOfDecimalPlaces);
            var UnitString = UnitList[UnitIndex];

            sb.Append(unitUnitsString);
            if (SpaceBetweenNumberAndUnit)
                sb.Append(" ");
            sb.Append(UnitString);

            return sb.ToString();
        }
        #endregion

        #region 得到合适单位到指定小单位或最小单位的最小单位数量转换为字符串

        /// <summary>
        /// 得到合适单位到指定小单位或最小单位的最小单位数量转换为字符串
        /// </summary>
        /// <param name="MinUnitNumber">最小单位数量</param>
        /// <returns></returns>
        public String GetString2(Decimal MinUnitNumber)
        {
            return GetString2(MinUnitNumber, false);
        }

        /// <summary>
        /// 得到合适单位到指定小单位或最小单位的最小单位数量转换为字符串
        /// </summary>
        /// <param name="MinUnitNumber">最小单位数量</param>
        /// <param name="SpaceBetweenNumberAndUnit">数字与单位之间是否加空格</param>
        /// <returns></returns>
        public String GetString2(Decimal MinUnitNumber, Boolean SpaceBetweenNumberAndUnit)
        {
            return GetString2(MinUnitNumber, UnitList.Count - 1, 0, SpaceBetweenNumberAndUnit);
        }

        /// <summary>
        /// 得到合适单位到指定小单位或最小单位的最小单位数量转换为字符串
        /// </summary>
        /// <param name="MinUnitNumber">最小单位数量</param>
        /// <param name="MinUnitIndex">小单位序号</param>
        /// <param name="SpaceBetweenNumberAndUnit">数字与单位之间是否加空格</param>
        /// <returns></returns>
        public String GetString2(Decimal MinUnitNumber, Int32 MinUnitIndex, Boolean SpaceBetweenNumberAndUnit)
        {
            return GetString2(MinUnitNumber, UnitList.Count - 1, MinUnitIndex, SpaceBetweenNumberAndUnit);
        }

        /// <summary>
        /// 得到合适单位到指定小单位或最小单位的最小单位数量转换为字符串
        /// </summary>
        /// <param name="MinUnitNumber">最小单位数量</param>
        /// <param name="MinUnit">小单位字符串</param>
        /// <param name="SpaceBetweenNumberAndUnit">数字与单位之间是否加空格</param>
        /// <returns></returns>
        public String GetString2(Decimal MinUnitNumber, String MinUnit, Boolean SpaceBetweenNumberAndUnit)
        {
            return GetString2(MinUnitNumber, null, MinUnit, SpaceBetweenNumberAndUnit);
        }

        /// <summary>
        /// 得到合适单位到指定小单位或最小单位的最小单位数量转换为字符串
        /// </summary>
        /// <param name="MinUnitNumber">最小单位数量</param>
        /// <param name="MaxUnit">大单位字符串</param>
        /// <param name="MinUnit">小单位字符串</param>
        /// <param name="SpaceBetweenNumberAndUnit">数字与单位之间是否加空格</param>
        /// <returns></returns>
        public String GetString2(Decimal MinUnitNumber, String MaxUnit, String MinUnit, Boolean SpaceBetweenNumberAndUnit)
        {
            var MaxUnitIndex = UnitList.Count - 1;
            if (!String.IsNullOrEmpty(MaxUnit))
            {
                MaxUnitIndex = UnitList.IndexOf(MaxUnit);
                if (MaxUnitIndex < 0)
                    throw new FormatException("在单位列表中未找到指定的大单位。");
            }
            var MinUnitIndex = UnitList.IndexOf(MinUnit);
            if (MinUnitIndex < 0)
                throw new FormatException("在单位列表中未找到指定的小单位。");

            return GetString2(MinUnitNumber, MaxUnitIndex, MinUnitIndex, SpaceBetweenNumberAndUnit);
        }

        /// <summary>
        /// 得到合适单位到指定小单位或最小单位的最小单位数量转换为字符串
        /// </summary>
        /// <param name="MinUnitNumber">最小单位数量</param>
        /// <param name="MaxUnitIndex">大单位序号</param>
        /// <param name="MinUnitIndex">小单位序号</param>
        /// <param name="SpaceBetweenNumberAndUnit">数字与单位之间是否加空格</param>
        /// <returns></returns>
        public String GetString2(Decimal MinUnitNumber, Int32 MaxUnitIndex, Int32 MinUnitIndex, Boolean SpaceBetweenNumberAndUnit)
        {
            StringBuilder sb = new StringBuilder();

            var TmpValue = MinUnitNumber;
            for (int i = MaxUnitIndex; i >= MinUnitIndex; i--)
            {
                var CurrentUnitUnits = Decimal.ToInt32(GetUnits(TmpValue, i));
                var CurrentUnitUnitsString = CurrentUnitUnits.ToString();
                var UnitString = UnitList[i];

                TmpValue = TmpValue - GetMinUnitUnits(CurrentUnitUnits, i);

                if (CurrentUnitUnits > 0 || sb.Length > 0 || i == MinUnitIndex)
                {
                    sb.Append(CurrentUnitUnitsString);
                    if (SpaceBetweenNumberAndUnit)
                        sb.Append(" ");
                    sb.Append(UnitString);
                    if (SpaceBetweenNumberAndUnit)
                        sb.Append(" ");
                }
            }
            if (SpaceBetweenNumberAndUnit && sb.Length > 0)
                sb.Length -= 1;
            return sb.ToString();
        }
        #endregion
    }
}