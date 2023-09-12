using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    EnemyCharacter _character;

    [SerializeField]Range _searchRange;
    [SerializeField]Range _attackRange;
    private float _movePacketDelay = 0.1f;

    Range AttackRange
    {
        get
        {
            Range temp = _attackRange;
            temp.center.x = transform.localScale.x > 0 ? temp.center.x : -temp.center.x;

            return temp;
        }
    }
    Range SearchRange
    {
        get
        {
            Range temp = _searchRange;
            temp.center.x = transform.localScale.x > 0 ? temp.center.x : -temp.center.x;

            return temp;
        }
    }

    private void Awake()
    {
        _character = GetComponent<EnemyCharacter>();
        StartCoroutine(CorSendMovePacket());
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + SearchRange.center, SearchRange.size);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + AttackRange.center, AttackRange.size);
    }

    private void Update()
    {
        AI();
    }

    void AI()
    {
        if (_character.Target != null && !_character.Target.gameObject.activeSelf) _character.Target = null;
        if (_character.IsAttacking) return;

        if (_character.CharacterState == CharacterState.Idle)
        {
            if (_character.Target == null)
            {
                Search();
            }
            else
            {
                PlayerCharacter character = Util.GetGameObjectByPhysics<PlayerCharacter>(transform.position, AttackRange, LayerMask.GetMask("Character"));
                Building building = Util.GetGameObjectByPhysics<Building>(transform.position, AttackRange, LayerMask.GetMask("Character"));

              
                if (character != null || building != null)
                {
                    _character.SetCharacterDirection(Vector2.zero);
                    _character.IsAttacking = true;
                    _character.CharacterState = CharacterState.Attack;
                    SendMoveData();
                }
                else
                {
                    _character.IsAttacking = false;
                    _character.SetCharacterDirection(_character.Target.transform.position - transform.position);
                }
            }
        }
        else
        {
            _character.SetCharacterDirection(Vector2.zero);
        }
    }


    void Search()
    {
        int layerMask = LayerMask.GetMask("Character");
        RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position + SearchRange.center, SearchRange.size, 0, Vector2.zero,0,layerMask);
        if (hits.Length <= 0) return;

        foreach(RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject == _character.gameObject) continue;
            if (hit.collider.gameObject.tag == "Player")
            {
                Character character = hit.collider.gameObject.GetComponentInParent<Character>();
                if (character != null)
                {
                    _character.Target = character;
                    break;
                }
            }
        }
    }

    IEnumerator CorSendMovePacket()
    {
        while (true)
        {

            SendMoveData();
            yield return new WaitForSeconds(_movePacketDelay);
        }
    }

    void SendMoveData()
    {
        C_Move packet = new C_Move();
        packet.characterId = _character.CharacterId;
        packet.posX = transform.position.x;
        packet.posY = transform.position.y;
        packet.posZ = transform.position.z;
        packet.currentSpeed = _character.CurrentSpeed;
        packet.ySpeed = _character.YSpeed;
        packet.characterState = (int)_character.CharacterState;
        packet.characterMoveDirection = _character.CharacterMoveDirection.x;
        packet.attackType = _character.AttackType;
        packet.isAttacking = _character.IsAttacking;
        packet.isJumping = _character.IsJumping;
        packet.isContactGround = _character.IsContactGround;
        packet.isConnectCombo = _character.IsConncetCombo;

        Client.Instance.Send(packet.Write());
    }
}
