using System.Collections;
using System.Collections.Generic;
using BluePro;
using BluePro.FrameWork;
using BluePro.Skill;
using UnityEngine;

public class SkillUtil
{
    public static string GetSkillDebugDes(ISkill skill)
    {
        return string.Format("[SkillId->{0} Name->{1} ]", skill.GetSkillId(), skill.GetData().Name);
    }

//---------------------------------------------------------------------------------------------------------//
    public static SkillActionData GetSkillActionData(int id)
    {
        var loaderService = ServiceLocate.Instance.GetService<LoaderService>();
        return loaderService.GetRowDataFromTable<SkillActionData>(SkillDefine.SKILL_ACTION_TABLE, id);
    }

    public static SkillBuffData GetSkillBuffData(int id)
    {
        var loaderService = ServiceLocate.Instance.GetService<LoaderService>();
        return loaderService.GetRowDataFromTable<SkillBuffData>(SkillDefine.SKILL_BUFF_TABLE, id);
    }

    public static SkillStateData GetSkillStateData(int id)
    {
        var loaderService = ServiceLocate.Instance.GetService<LoaderService>();
        return loaderService.GetRowDataFromTable<SkillStateData>(SkillDefine.SKILL_STATE_TABLE, id);
    }

//---------------------------------------------------------------------------------------------------------//
    public static float GetBuffDurationTime(int lv, List<float> duration)
    {
        if (duration.Count == 1)
            return duration[0];
        int index = GetIndexByLevel(lv);
        if (IsValidIndex(index, duration))
            return duration[index];
        return 0;
    }

    public static int GetSkillCost(int lv, SkillData data)
    {
        if (data == null)
        {
            LogError("data is null");
            return 0;
        }

        int index = GetIndexByLevel(lv);
        if (!IsValidIndex(index, data.SkillCost))
            return 0;

        return data.SkillCost[index];
    }


    public static float GetSkillCoolDown(int lv, SkillData data)
    {
        if (data == null)
        {
            LogError("data is null");
            return 0;
        }

        int index = GetIndexByLevel(lv);
        if (!IsValidIndex(index, data.CoolDown))
            return 0f;

        return data.CoolDown[index];
    }

    public static float GetSkillCastPoint(int lv, SkillData data)
    {
        if (data == null)
        {
            LogError("data is null");
            return 0;
        }

        int index = GetIndexByLevel(lv);
        if (!IsValidIndex(index, data.SkillCastPoint))
            return 0f;

        return data.SkillCastPoint[index];
    }

    public static bool GetSkillActionDamage(int lv, SkillActionData data, out int damage)
    {
        return GetSkillActionParamFromList(lv, data, out damage);
    }

    public static bool GetSkillActionHeal(int lv, SkillActionData data, out int heal)
    {
        return GetSkillActionParamFromList(lv, data, out heal);
    }

    private static bool GetSkillActionParamFromList(int lv, SkillActionData data, out int result)
    {
        result = 0;
        if (data == null)
        {
            LogError("data is null");
            return false;
        }

        if (GetParamFromList(lv, data.Para1, out result))
            return true;
        return false;
    }

    private static bool GetParamFromList(int lv, string param, out int result)
    {
        result = 0;
        int index = GetIndexByLevel(lv);
        List<int> list = ConvertUtil.ConvertToIntList(param);

        if (list == null)
        {
            Debug.LogError("集合为null");
            return false;
        }

        if (!IsValidIndex(index, list))
        {
            Debug.LogError(string.Format("索引溢出 Index->{0} ", index));
            return false;
        }

        result = list[index];
        return true;
    }

    private static bool IsValidIndex(int index, IList list)
    {
        var result = index >= 0 && index < list.Count;
        if (!result)
            LogError(string.Format("Invalid Level  index->{0}  Total List Count->{1}", index, list.Count));
        return result;
    }

    private static int GetIndexByLevel(int level)
    {
        return level - 1;
    }

//---------------------------------------------------------------------------------------------------------//
    public static bool TryParseWithDebug(string param, out int intParam, string error)
    {
        if (int.TryParse(param, out intParam))
            return true;

        SkillUtil.LogError(string.Format("{0}   [策划配置错误 {1}]", param, error));
        return false;
    }

    public static bool TryParseWithDebug(string param, out float intParam, string error)
    {
        if (float.TryParse(param, out intParam))
            return true;

        SkillUtil.LogError(string.Format("{0}   [策划配置错误 {1}]", param, error));
        return false;
    }

//---------------------------------------------------------------------------------------------------------//

    private static string[] effects =
    {
        "Demo/Missile Prefabs/Fireball/FireBallPinkOBJ",
        "Demo/Missile Prefabs/Frost/FrostMissileOBJ",
        "Demo/Missile Prefabs/Rocket/RocketBlueOBJ",
        "Demo/Missile Prefabs/MagicSoft/MagicSoftFireOBJ",
        "Demo/Missile Prefabs/Mystic/MysticOrangeOBJ",
        "Demo/Missile Prefabs/Storm/StormMissileOBJ",
        "Demo/Missile Prefabs/EnergyNova/NovaFireOBJ",
    };

    public static void CreatProjectile(ISkill skill, CommonParam param, SkillActionData effectData)
    {
        string effectName = effectData.Para1;
        TryParseWithDebug(effectData.Para2, out int speed, "投射物速度");
        string startPoint = effectData.Para3;

        int index = UnityEngine.Random.Range(0, effects.Length);
        effectName = effects[index]; //todo FakeData

        Transform spawnTransform = null;
        if (param.IsBounce && param.BounceParam.BounceTime > 1) //技能发出点是上一个Target
            spawnTransform = param.BounceParam.LastBouncedActor.GetTransform();
        else //技能发出点是默认的技能释放者
            spawnTransform = skill.GetContext().GetSelfActor().GetTransform();

        var newTarget = param.Targets[0];
        if (param.IsBounce)
            param.BounceParam.AddLastBouncedTarget(newTarget);

        Transform spawnPoint = spawnTransform.Find(startPoint);
        Vector3 spawnPosition = spawnPoint ? spawnPoint.position : spawnTransform.position;

        GameObject projectile = EffectUtil.InstantiateEffect(effectName, spawnPosition, Quaternion.identity);
        var script = projectile.AddComponent<SkillProjectile>();
        script.BeginProjectile(skill, param, effectData, newTarget, speed);
    }

    //---------------------------------------------------------------------------------------------------------//
    public static float GetRandomValue()
    {
        return UnityEngine.Random.value;
    }

    //---------------------------------------------------------------------------------------------------------//
    public static void Log(string log)
    {
        Debuger.Log(log);
    }

    public static void LogWarning(string log)
    {
        Debuger.LogWarning(log);
    }

    public static void LogError(string log)
    {
        Debuger.LogError(log);
    }
}