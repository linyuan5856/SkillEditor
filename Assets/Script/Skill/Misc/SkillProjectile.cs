using BluePro.Skill;
using UnityEngine;

public class SkillProjectile : MonoBehaviour
{
    public GameObject impactParticle;
    public GameObject projectileParticle;
    public GameObject muzzleParticle;

    [Header("Adjust if not using Sphere Collider")]
    public float colliderRadius = 1f;

    [Range(0f, 1f)] public float collideOffset = 0.15f;

    private Rigidbody mRigidBody;
    private SphereCollider mCollider;

    private ISkill mSkill;
    private CommonParam mParam;
    private ISkillActor mActor;
    private SkillActionData mData;
    private float mSpeed;
    private Vector3 casterPosition; //发射点坐标

    private float startTime;
    private float MAXTIME = 8.0f;

    //********跟踪投射物参数*******//
    private bool mIsFollowProjectile;
    private bool mCanBlink;

    //******线性投射物参数********//
    private bool mDeleteAfterHit;
    private float mMinRadiusRange;
    private float mMaxRadiusRange;
    private float mMaxDistance;
    private Vector3 lastDir;

    void Awake()
    {
        //todo will delete in the future
//        var script = this.GetComponent<ETFXProjectileScript>();
//        if (script)
//        {
//            script.enabled = false;
//            this.impactParticle = script.impactParticle;
//            this.projectileParticle = script.projectileParticle;
//            this.muzzleParticle = script.muzzleParticle;
//        }

        mRigidBody = GetComponent<Rigidbody>();
        mRigidBody.useGravity = false;

        mCollider = GetComponent<SphereCollider>();
        if (mCollider)
            mCollider.isTrigger = true;

        if (projectileParticle)
        {
            projectileParticle = Instantiate(projectileParticle, transform.position, transform.rotation);
            projectileParticle.transform.parent = transform;
            var collider = projectileParticle.GetComponent<Collider>();
            if (collider)
                collider.isTrigger = true;
            // SetOrder(projectileParticle.transform);
        }

        if (muzzleParticle)
        {
            muzzleParticle = Instantiate(muzzleParticle, transform.position, transform.rotation);
            Destroy(muzzleParticle, 1.5f);
            var collider = muzzleParticle.GetComponent<Collider>();
            if (collider)
                collider.isTrigger = true;
            // SetOrder(muzzleParticle.transform);
        }
    }

    public void BeginProjectile(ISkill skill, CommonParam param, SkillActionData data, ISkillActor actor, float speed)
    {
        if (skill == null || param == null || data == null || actor == null)
        {
            SkillUtil.LogError("Null Param");
            return;
        }

        this.mSkill = skill;
        this.mParam = param;
        this.mActor = actor;
        this.mData = data;
        this.startTime = Time.unscaledTime;
        var type = (SkillActionType) data.ActionType;
        this.mIsFollowProjectile = type == SkillActionType.FollowProjectile;
        this.lastDir = Vector3.zero;
        this.mSpeed = speed;
        this.casterPosition = mSkill.GetContext().GetSelfActor().GetTransform().position;

        if (this.mIsFollowProjectile)
        {
            mCanBlink = data.Para4 == "1";
        }
        else
        {
            mDeleteAfterHit = data.Para4 == "1";
            SkillUtil.TryParseWithDebug(data.Para5, out mMinRadiusRange, "线性投射起始范围");
            SkillUtil.TryParseWithDebug(data.Para5, out mMaxRadiusRange, "线性投射结束范围");
            SkillUtil.TryParseWithDebug(data.Para5, out mMaxDistance, "线性投射固定距离");
        }

        // this.mSpeed = 350; //low speed for test
        //Debug.LogError("SetTarget->" + mActor.GetTransform().name);
        this.transform.LookAt(mActor.GetTransform().position);
        mRigidBody.AddForce(this.transform.forward * this.mSpeed);
    }

    RaycastHit[] hits = new RaycastHit[6];
    private RaycastHit hit;

    void FixedUpdate()
    {
        if (!mRigidBody)
            return;


        var diff = Time.unscaledTime - startTime;
        if (diff >= MAXTIME) //todo 按照距离还是时间 或者两个都支持有待商榷
        {
            this.DestroyProjectile(true);
            return;
        }

        if (!mIsFollowProjectile && Vector3.Distance(casterPosition, transform.position) >= mMaxDistance)
        {
            this.DestroyProjectile(true);
        }


        if (mIsFollowProjectile)
            FollowTarget();

        var velocity = mRigidBody.velocity;
        Vector3 direction = velocity.normalized;
        float detectionDistance =
            velocity.magnitude * Time.deltaTime;
        float radius = GetRadius();
        radius = 0.05f; //todo 配置表中半径太大 无法命中了


        if (!mIsFollowProjectile)
        {
            if (Physics.SphereCast(transform.position, radius, direction, out hit, detectionDistance))
                OnHit(hit);
        }
        else
        {
            //todo  命中方式有待商榷
            var size = Physics.SphereCastNonAlloc(transform.position, radius, direction, hits, detectionDistance);
            if (size > 0)
            {
                //  Debug.LogError("------------------Hit  Start--------------------" );
                for (int i = 0; i < hits.Length; i++)
                {
                    var hit = hits[i];
                    //Debug.LogError("------------------Hit--------------------" + hit.transform);
                    //todo 临时写法
                    if (hit.transform != this.mActor.GetTransform())
                        continue;
                    OnHit(hit);
                    break;
                }

                // Debug.LogError("------------------Hit  End--------------------" );
            }
        }
    }

    void OnHit(RaycastHit hit)
    {
        transform.position = hit.point + (hit.normal * collideOffset);
        if (impactParticle)
        {
            GameObject impactP = Instantiate(impactParticle, transform.position,
                Quaternion.FromToRotation(Vector3.up, hit.normal));
            var collider = impactP.GetComponent<Collider>();
            if (collider)
                collider.isTrigger = true;
            Destroy(impactP, 3.5f);
        }

        if (mDeleteAfterHit || mIsFollowProjectile)
            DestroyProjectile();
        if (!mIsFollowProjectile) //非指定目标
        {
            var actor = hit.transform.GetComponent<ISkillActor>();
            if (actor != null)
            {
                mParam.ResetTarget();
                mParam.AddTarget(actor);
            }
        }


        if (mSkill != null && mParam != null)
        {
            mSkill.OnProjectileHit(mParam.IdentifyId);
            mSkill.OnBounceNodeEnd(mParam.IdentifyId, mData.Id);

            //1.不在弹射状态才调用，弹射状态由OnBounceNodeEnd函数内部去调用RemoveAction
            //2.弹射可能会因为超过最大弹射次数而提前结束技能(此时弹射状态已经重置)
            if (!mParam.IsBounce && !mSkill.IsSKillEnd(mParam.IdentifyId))
                mSkill.RemoveSkillAction(mData.Id, mParam);
        }
    }

    float GetRadius()
    {
        float radius = 0.0f;
        if (!mIsFollowProjectile) //线性投射，半径慢慢递增
        {
            if (radius < mMinRadiusRange)
                radius = mMinRadiusRange;

            radius = Mathf.Lerp(radius, mMaxRadiusRange, Time.deltaTime);
        }
        else
        {
            radius = mCollider ? mCollider.radius : colliderRadius;
        }

        return radius;
    }

    void FollowTarget()
    {
        var targetPos = mActor.GetTransform().position;
        var dir = targetPos - casterPosition;
        if (dir != lastDir)
        {
            this.transform.LookAt(targetPos);
            mRigidBody.velocity = Vector3.zero;
            mRigidBody.AddForce(transform.forward * mSpeed);
        }

        lastDir = dir;
    }

    void DestroyProjectile(bool isBreak = false)
    {
        ParticleSystem[] trails = GetComponentsInChildren<ParticleSystem>();

        for (int i = 1; i < trails.Length; i++)
        {
            ParticleSystem trail = trails[i];

            if (trail.gameObject.name.Contains("Trail"))
            {
                trail.transform.SetParent(null);
                Destroy(trail.gameObject, 2f);
            }
        }

        Destroy(projectileParticle, 3f);
        Destroy(gameObject);

        if (isBreak && mSkill != null && mParam != null)
        {
            mSkill.OnProjectileDisappear(mParam.IdentifyId);
            mSkill.RemoveSkillAction(mData.Id, mParam);
        }
    }

//    void SetOrder(Transform t)
//    {
//        var renedrs = t.GetComponentsInChildren<Renderer>();
//        for (int i = 0; i < renedrs.Length; i++)
//        {
//            var render = renedrs[i];
//            render.sortingOrder = 1;
//        }
//    }
}