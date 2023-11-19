using System.Collections;
using UnityEngine;
using static Define;

public class CustomWeapon : Weapon
{
    [SerializeField] protected CustomCharacter _customCharacter;
    protected CharacterEquipment _characterEquipment;
    protected PlayerController _playerController;
    protected HelperAI _helperAi;

    public override bool IsEnableAttack => _attackTime >= AttackData.attackDelay;

    ItemAmmo _itemAmmo;

    public WeaponData WeaponData => Manager.Data.GetWeaponData(_characterEquipment.EquipWeaponName);
    public override AttackData AttackData => WeaponData.AttackList[0];

    public override void Awake()
    {
        
    }

    public virtual void Init()
    {
        _character = GetComponentInParent<Character>();
        _animatorHandler = GetComponentInParent<AnimatorHandler>();


        _customCharacter = GetComponentInParent<CustomCharacter>();
        if (_customCharacter != null)
        {
            _characterEquipment = _customCharacter.GetComponent<CharacterEquipment>();
            _characterEquipment.EquipmentChanged += () =>
            {
                ItemAmmo itemAmmo = null;
                Item item = Manager.Item.GetItem(_characterEquipment.WeaponId);
                if (item != null)
                {
                    itemAmmo = item.GetComponent<ItemAmmo>();
                }
                _itemAmmo = itemAmmo;
            };
        }

        _playerController = GetComponentInParent<PlayerController>();
        _helperAi = GetComponentInParent<HelperAI>();

        RegisterControl();

        IsDone = true;
    }

    protected override void RegisterControl()
    {
        base.RegisterControl();

        _animatorHandler.AttackStartedHandler += OnAttackStart;

        if (_playerController != null)
        {
            _playerController.AttackKeyDownHandler += OnAttackKeyDown;
            _playerController.AttackKeyUpHandler += OnAttackKeyUp;
        }
        else if (_helperAi != null)
        {
            _helperAi.AttackHanlder += OnAttackKeyDown;
        }
    }
    protected override void UnregisterControl()
    {
        base.UnregisterControl();
        if (_playerController != null)
        {
            _playerController.AttackKeyDownHandler -= OnAttackKeyDown;
            _playerController.AttackKeyUpHandler -= OnAttackKeyUp;
        }
        else if (_helperAi != null)
        {
            _helperAi.AttackHanlder -= OnAttackKeyDown;
        }
        _animatorHandler.AttackStartedHandler -= OnAttackStart;

    }

    protected override void OnDrawGizmos()
    {
        if (_customCharacter == null && _character == null) return;

        Gizmos.color = Color.red;

        WeaponData weaponData = null;
        if (_customCharacter != null)
            weaponData = WeaponData;

        AttackData attackData = weaponData == null ? _defaultAttack : weaponData.AttackList[0];

        if (_customCharacter != null)
        {
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(_customCharacter.FrontWeapon.transform.position, _customCharacter.FrontWeapon.transform.rotation, _customCharacter.FrontWeapon.transform.localScale);
            Gizmos.matrix = rotationMatrix;
        }

        if (_character != null)
            attackData.attackRange.center.x = (_character?.gameObject.transform.localScale.x > 0 ? attackData.attackRange.center.x : -attackData.attackRange.center.x);

        Vector3 point = attackData.attackEffectPoint;

        Gizmos.DrawWireSphere(point, 0.1f);

        if (attackData.projectile)
        {
            Gizmos.DrawWireSphere(attackData.firePos, 0.1f);
        }

        if (_customCharacter != null)
        {
            switch (attackData.attacKShape)
            {
                case Define.AttacKShape.Rectagle:
                    Gizmos.DrawWireCube(attackData.attackRange.center, attackData.attackRange.size);
                    break;
                case Define.AttacKShape.Circle:
                    Gizmos.DrawWireSphere(_customCharacter.FrontWeapon.transform.position + attackData.attackRange.center, attackData.attackRange.size.x);
                    break;
                case Define.AttacKShape.Raycast:
                    Gizmos.DrawRay(attackData.attackRange.center, (_character == null ? Vector3.right : _character.transform.localPosition.x > 0 ? Vector3.right : Vector3.left) * attackData.attackRange.size.x);
                    break;
            }
        }
        else
        {
            switch (attackData.attacKShape)
            {
                case Define.AttacKShape.Rectagle:
                    Gizmos.DrawWireCube(transform.position + attackData.attackRange.center, attackData.attackRange.size);
                    break;
                case Define.AttacKShape.Circle:
                    Gizmos.DrawWireSphere(transform.position + attackData.attackRange.center, attackData.attackRange.size.x);
                    break;
                case Define.AttacKShape.Raycast:
                    Gizmos.DrawRay(transform.position, (_character == null ? Vector3.right : _character.transform.localPosition.x > 0 ? Vector3.right : Vector3.left) * attackData.attackRange.size.x);
                    break;
            }
        }
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, attackData.AttackDirection.normalized);
    }

    protected override void OnAttackKeyDown()
    {
        if (Manager.Building.IsDrawing) return;
        if (_customCharacter != null && (_customCharacter.HoldingItem != null && !_customCharacter.IsEquipWeapon)) return;
        if (AttackData.attackDelay > _attackTime) return;
        if (AttackData.projectile != null && _itemAmmo != null && _itemAmmo.currentAmmo <= 0) return;

        _attackTime = 0;
        _character.IsAttacking = true;
        _character.IsEnableMoveWhileAttack = true;
        _character.AttackType = _attackType;

        GameObject attackEffect = Manager.Data.GetEffect(AttackData.attackEffectName);

        if (attackEffect != null)
        {
            GameObject effect = Instantiate(attackEffect);

            Vector3 point = AttackData.attackEffectPoint;
            point.x *= _character.transform.localScale.x > 0 ? 1 : -1;

            effect.transform.position = transform.position + point;
            Vector3 scale = Vector3.one;
            scale.x = _character.transform.localScale.x > 0 ? 1 : -1;
            effect.transform.localScale = scale;
        }
        Client.Instance.SendCharacterControlInfo(_character);
    }


    public override void Attack()
    {
        // 특수 공격
        if (AttackData.actualAttackEffectName != Define.EffectName.None)
        {
            int layerMask = gameObject.tag == "Enemy" ? Define.PlayerLayerMask : Define.EnemyLayerMask;
            Character character = Util.GetGameObjectByPhysics<Character>(transform.position, AttackData.attackRange, layerMask);

            if (character != null)
            {
                GameObject effect = null;
                Manager.Effect.GenerateEffect(AttackData.actualAttackEffectName, character.transform.position, ref effect);
            }
        }
        // 근접 공격
        else if (AttackData.projectile == null)
        {
            Range attackRange = AttackData.attackRange;
            attackRange.center.x = (_character.gameObject.transform.localScale.x > 0 ? AttackData.attackRange.center.x : -AttackData.attackRange.center.x);

            int layerMask = 0;
            if (_character.tag.Equals(Define.CharacterTag.Player.ToString()))
            {
                layerMask = Define.EnemyLayerMask | Define.BuildingLayerMask;
            }
            else if (_character.tag.Equals(Define.CharacterTag.Enemy.ToString()))
            {
                layerMask = Define.PlayerLayerMask | Define.BuildingLayerMask;
            }

            RaycastHit2D[] hits = null;
            if (_customCharacter != null)
            {
                Matrix4x4 rotationMatrix = Matrix4x4.TRS(_customCharacter.FrontWeapon.transform.position, _customCharacter.FrontWeapon.transform.rotation, _customCharacter.FrontWeapon.transform.localScale);
                Vector3 pos = rotationMatrix.MultiplyVector(attackRange.center);

                switch (AttackData.attacKShape)
                {
                    case Define.AttacKShape.Rectagle:

                        hits = Physics2D.BoxCastAll(_customCharacter.FrontWeapon.transform.position + pos, attackRange.size, _customCharacter.FrontWeapon.transform.eulerAngles.z, Vector2.zero, 0, layerMask);
                        break;
                    case Define.AttacKShape.Raycast:
                        hits = Physics2D.RaycastAll(_customCharacter.FrontWeapon.transform.position + pos, transform.parent.localScale.x > 0 ? Vector2.right : Vector2.left, attackRange.size.x, layerMask);
                        break;
                    default:
                        hits = Physics2D.BoxCastAll(_customCharacter.FrontWeapon.transform.position + pos, attackRange.size, _customCharacter.FrontWeapon.transform.eulerAngles.z, Vector2.zero, 0, layerMask);
                        break;
                }
            }

            int penetration = 0;

            if (hits.Length > 0)
            {
                foreach (RaycastHit2D hit in hits)
                {
                    Character character = hit.collider.GetComponent<Character>();
                    Building building = hit.collider.GetComponent<Building>();

                    if (character != null && character != _character)
                    {
                        Camera.main.GetComponent<CameraMove>().ShakeCamera(AttackData.power, 0.4f);

                        if (Client.Instance.ClientId == -1 || Client.Instance.IsMain)
                        {
                            Item item = Manager.Item.GetItem(_characterEquipment.WeaponId);
                            ItemEquipment itemEquipment = null;
                            if (item != null)
                                itemEquipment = item.GetComponent<ItemEquipment>();

                            Vector3 attackDirection = AttackData.AttackDirection;
                            attackDirection.x = _character.transform.localScale.x > 0 ? attackDirection.x : -attackDirection.x;
                            character.Damage(AttackData.damage + (itemEquipment != null? itemEquipment.AddedAttack:0), attackDirection, AttackData.power, AttackData.stagger);

                            Vector3 point = hit.point;

                            GameObject effect = null;
                            Manager.Effect.GenerateEffect(AttackData.hitEffectName, point, ref effect);
                        }
                        penetration++;
                        if (penetration > AttackData.penetrationPower) break;
                    }
                    else if (building != null && (_character.tag.Equals(CharacterTag.Player.ToString()) && building.tag.Equals(CharacterTag.Enemy.ToString())
                        || _character.tag.Equals(CharacterTag.Enemy.ToString()) && !building.tag.Equals(CharacterTag.Enemy.ToString())))
                    {

                        if (Client.Instance.ClientId == -1 || Client.Instance.IsMain)
                        {
                            Item item = Manager.Item.GetItem(_characterEquipment.WeaponId);
                            ItemEquipment itemEquipment = null;
                            if(item != null)
                                itemEquipment = item.GetComponent<ItemEquipment>();

                            building.Damage(AttackData.damage + (itemEquipment != null ? itemEquipment.AddedAttack : 0));

                            Vector3 point = hit.point;

                            GameObject effect = null;
                            Manager.Effect.GenerateEffect(AttackData.hitEffectName, point, ref effect);
                        }
                        penetration++;
                        if (penetration > AttackData.penetrationPower) break;
                    }
                }

            }
        }
        // 원거리 공격
        else
        {
            if (_customCharacter != null)
            {
                float rotation = 0;
                Vector3 firePoint = Vector3.zero;
                if ((_customCharacter.WeaponData != null && _customCharacter.WeaponData.IsFrontWeapon) || _customCharacter.WeaponData == null)
                {
                    _customCharacter.FrontfirePoint.transform.localPosition = AttackData.firePos;
                    firePoint = _customCharacter.FrontfirePoint.transform.position;
                    rotation = _customCharacter.GetFrontHandRotation();
                }
                else if (_customCharacter.WeaponData != null && !_customCharacter.WeaponData.IsFrontWeapon)
                {
                    _customCharacter.BehindfirePoint.transform.localPosition = AttackData.firePos;
                    firePoint = _customCharacter.BehindfirePoint.transform.position;
                    rotation = _customCharacter.GetBehindHandRotation();
                }
                Vector3 direction = new Vector3(Mathf.Cos(rotation * Mathf.Deg2Rad) *_customCharacter.transform.localScale.x, Mathf.Sin(rotation * Mathf.Deg2Rad)).normalized;
                Projectile projectile = null;

                CharacterTag tag1 = CharacterTag.Enemy;
                CharacterTag tag2 = CharacterTag.Enemy;

                if (_customCharacter.tag == CharacterTag.Player.ToString())
                {
                    tag1 = CharacterTag.Enemy;
                    tag2 = CharacterTag.Enemy;
                }
                else if (_customCharacter.tag == CharacterTag.Enemy.ToString())
                {
                    tag1 = CharacterTag.Player;
                    tag2 = CharacterTag.Building;
                }
                Item item = Manager.Item.GetItem(_characterEquipment.WeaponId);
                ItemEquipment itemEquipment = null;
                if (item != null)
                    itemEquipment = item.GetComponent<ItemEquipment>();

                Manager.Projectile.SetPacketDetail(direction, tag1, tag2, AttackData.damage + (itemEquipment != null ? itemEquipment.AddedAttack : 0));
                Manager.Projectile.GenerateProjectile(AttackData.projectile.ProjectileName, firePoint, ref projectile);

                projectile?.Fire(direction, tag1, tag2, AttackData.damage + (itemEquipment != null ? itemEquipment.AddedAttack : 0));

                if (_itemAmmo != null)
                {
                    //_itemAmmo.currentAmmo--;
                }
            }
        }
    }
}
