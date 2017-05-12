using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using static ZeldaTimelineGeneratorUtility.EnumBindingSourceExtension;

namespace ZeldaTimelineGeneratorUtility
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum GameEnum
    {
        [Description("No Data")]
        NoData = 0,
        [Description("The Legend of Zelda")]
        TheLegendOfZelda,
        [Description("The Adventure of Link")]
        Zelda2TheAdventureofLink,
        [Description("A Link to the Past")]
        ALinkToThePast,
        [Description("Ancient Stone Tablets")]
        BSZeldaAncientStoneTablets,
        [Description("Link's Awakening")]
        LinksAwakening,
        [Description("Ocarina of Time")]
        OcarinaofTime,
        [Description("Majora's Mask")]
        MajorasMask,
        [Description("Oracle of Ages/Seasons")]
        OoX,
        [Description("The Wind Waker")]
        TheWindWaker,
        [Description("Four Swords")]
        FourSwords,
        [Description("Four Swords Adventures")]
        FourSwordsAdventures,
        [Description("(Wii) Twilight Princess")]
        TwilightPrincess,
        [Description("(GCN) Twilight Princess")]
        TwilightPrincessGCN,
        [Description("The Minish Cap")]
        TheMinishCap,
        [Description("Skyward Sword")]
        SkywardSword,
        [Description("Phantom Hourglass")]
        PhantomHourglass,
        [Description("Spirit Tracks")]
        SpiritTracks,
        [Description("A Link Between Worlds")]
        ALinkBetweenWorlds,
        [Description("Triforce Heroes")]
        TriForceHeroes,
        [Description("Breath of the Wild")]
        BreathOfTheWild,
        [Description("Link's Awakening DX")]
        LinksAwakeningDX,
        [Description("(MQ) Ocarina of Time")]
        OcarinaOfTimeMasterQuest,
        [Description("(3DS) Ocarina of Time")]
        OcarinaOfTime3D,
        [Description("(3DS) Majora's Mask")]
        MajorasMask3D,
        [Description("(WiiU) The Wind Waker")]
        TheWindWakerHD,
        [Description("(WiiU) Twilight Princess")]
        TwilightPrincessHD,
        [Description("Wand of Gamelon")]
        ZeldaWandofGamelon,
        [Description("Faces of Evil")]
        LinkTheFacesOfEvil,
        [Description("Zelda's Adventure")]
        ZeldasAdventure,
        [Description("Crossbow Training")]
        LinksCrossbowTraining,
        [Description("Hyrule Warriors")]
        HyruleWarriors,
        [Description("Hyrule Warriors Legends")]
        HyruleWarriorsLegends
    }

    public static class GameEnumDescription
    {
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }

    public class EnumBindingSourceExtension : MarkupExtension
    {
        private Type _enumType;
        public Type EnumType
        {
            get { return this._enumType; }
            set
            {
                if (value != this._enumType)
                {
                    if (null != value)
                    {
                        Type enumType = Nullable.GetUnderlyingType(value) ?? value;
                        if (!enumType.IsEnum)
                            throw new ArgumentException("Type must be for an Enum.");
                    }

                    this._enumType = value;
                }
            }
        }

        public EnumBindingSourceExtension() { }

        public EnumBindingSourceExtension(Type enumType)
        {
            this.EnumType = enumType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (null == this._enumType)
                throw new InvalidOperationException("The EnumType must be specified.");

            Type actualEnumType = Nullable.GetUnderlyingType(this._enumType) ?? this._enumType;
            Array enumValues = Enum.GetValues(actualEnumType);

            if (actualEnumType == this._enumType)
                return enumValues;

            Array tempArray = Array.CreateInstance(actualEnumType, enumValues.Length + 1);
            enumValues.CopyTo(tempArray, 1);
            return tempArray;
        }

        public class EnumDescriptionTypeConverter : EnumConverter
        {
            public EnumDescriptionTypeConverter(Type type)
                : base(type)
            {
            }

            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                if (destinationType == typeof(string))
                {
                    if (value != null)
                    {
                        FieldInfo fi = value.GetType().GetField(value.ToString());
                        if (fi != null)
                        {
                            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                            return ((attributes.Length > 0) && (!String.IsNullOrEmpty(attributes[0].Description))) ? attributes[0].Description : value.ToString();
                        }
                    }
                    return string.Empty;
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }
}
