using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class LunaDex : MonoBehaviour
{
    public enum Locations
    {
        Battle = 1,
        TestOverworld = 2
    }

    public enum LunenEnum
    {
        Barix,
        Debero,
        Drole,
        Ebako,
        Flamela,
        Flareo,
        Fleor,
        Furo,
        Futetra,
        Glacus,
        Icifiz,
        Kyoske,
        NulvaIncarnate,
        Rizo,
        Rootle,
        SirDungeness,
        Stuimp,
        Tabela,
        Wavi,
        Wavineer,
        Wavulus,
        Woufa,
        Wynern,
        Yibalis,
        zz_TestMonster
    }
    public enum ActionEnum
    {
        _NoMove,
        Allergens,
        AncientStorm,
        AttackBoost,
        Bite,
        Brimstone,
        BuffMirror,
        ChillAura,
        ClawCrush,
        Cleanse,
        CloseCombat,
        Confusion,
        CorruptionAura,
        CorruptionBlast,
        CorruptionSurge,
        CosmicThrust,
        DefenseBoost,
        DrillPeck,
        Enkindle,
        Execute,
        FinalChill,
        Fortify,
        Frenzy,
        FrontlineTank,
        FrozenHellfire,
        FrozenShell,
        Headwind,
        HealingAura,
        HighVoltageSlash,
        Hysteria,
        IceStorm,
        Incinerate,
        Intimidate,
        LeafBlower,
        Leech,
        Melt,
        MindWave,
        MorningWood,
        NullificationArmor,
        NulvaBeam,
        PlasmaArmor,
        PsyBurst,
        Refreeze,
        Retribution,
        RockSlide,
        RockStream,
        Root,
        RulerOfTheSea,
        Sacrifice,
        Scorch,
        ScorchingWater,
        SeaStorm,
        ShiftTactics,
        ShockingGaze,
        SourceShift,
        SpikeArmor,
        StarShower,
        StarStrike,
        SteelStrike,
        Streamline,
        SuperFang,
        SynchronizedArmor,
        TableFlip,
        Tackle,
        Tailwind,
        TidalShift,
        TidalWave,
        Torch,
        Trick,
        Tsunami,
        UnnervingWill,
        VineManipulation,
        VineSlash,
        Warcry,
        WaterVortex,
        WindKnives,
        WindVortex,
        Wrap,
        zz_DebugMove,
        Fuel
    }
    public enum StatusEffectEnum
    {
        Bleeding,
        BrokenArmor,
        Burned,
        Confused,
        Constrained,
        Corruptible,
        Freezing,
        Grounded,
        Oiled,
        Pacified,
        Reversed,
        Sanctuary,
        Shocked,
        Staggered,
        WeakMinded
    }
    

    public bool DEBUG;
    public bool ShowTemplates;
    [ConditionalField(nameof(ShowTemplates))] public GameObject LunenTemplate;
    [ConditionalField(nameof(ShowTemplates))] public GameObject MonsterTemplate;
    [ConditionalField(nameof(ShowTemplates))] public GameObject ActionTemplate;
    [ConditionalField(nameof(ShowTemplates))] public GameObject EffectTemplate;
    [EnumNamedArray(typeof(LunenEnum))]
    public List<GameObject> LunenList = new List<GameObject>();
    [EnumNamedArray(typeof(ActionEnum))]
    public List<GameObject> ActionList = new List<GameObject>();
    [EnumNamedArray(typeof(StatusEffectEnum))]
    public List<GameObject> StatusEffectList = new List<GameObject>();

    public GameObject GetLunenObject(LunenEnum lunen)
    {
        return LunenList[(int)lunen];
    }

    public Lunen GetLunen(LunenEnum lunen)
    {
        return LunenList[(int)lunen].GetComponent<Lunen>();
    }

    public GameObject GetActionObject(ActionEnum action)
    {
        return ActionList[(int)action];
    }

    public Action GetAction(ActionEnum action)
    {
        return ActionList[(int)action].GetComponent<Action>();
    }

    public GameObject GetStatusEffectObject(StatusEffectEnum effect)
    {
        return StatusEffectList[(int)effect];
    }

    public Effects GetStatusEffect(StatusEffectEnum effect)
    {
        return StatusEffectList[(int)effect].GetComponent<Effects>();
    }
}
