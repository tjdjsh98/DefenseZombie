using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ProjectileManager : MonoBehaviour
{
    static int projectileId = 0;

    Dictionary<int,Projectile > _projectileDictioanry= new Dictionary<int,Projectile>();

    Vector3 _direction;
    CharacterTag _tag1;
    CharacterTag _tag2;

    public void Init()
    {

    }

    public Projectile GetProjectile(int id)
    {
        if(_projectileDictioanry.TryGetValue(id, out var projectile))
        {
            return projectile;
        }
        return null;
    }


    public void SetPacketDetail(Vector3 direction, CharacterTag tag1, CharacterTag tag2)
    {
        _direction = direction;
        _tag1 = tag1;
        _tag2 = tag2;
    }


    // 싱글, 멀티 이펙트 생성
    public int GenerateProjectile(ProjectileName projectileNmae, Vector3 pos , ref Projectile projectile)
    {
        int requestNumber = 0;
        if (Client.Instance.IsSingle)
        {
            projectile = GenerateProjectile(projectileNmae, pos);
        }
        else
        {
            requestNumber = RequestGenerateProjectile(projectileNmae, pos);
        }

        return requestNumber;
    }

    // 싱글일 때
    private Projectile GenerateProjectile(ProjectileName projectileNmae, Vector3 pos)
    {
        Projectile projectileOrigin = Manager.Data.GetProjectile(projectileNmae);

        if (projectileOrigin == null) return null;

        Projectile projectile = Instantiate(projectileOrigin.gameObject).GetComponent<Projectile>();
        projectile.ProjectileId = ++projectileId;
        projectile.transform.position = pos;

        _projectileDictioanry.Add(projectile.ProjectileId,projectile);

        return projectile;
    }

    // 멀티 일 때 생성

    private int RequestGenerateProjectile(ProjectileName projectileNmae, Vector3 pos)
    {
        int requestNumber = Random.Range(100, 1000);

        Client.Instance.SendRequestGenreateProjectile(projectileNmae, pos, requestNumber,_direction,_tag1,_tag2);

        return requestNumber;
    }

    // 싱글, 멀티 혼용으로 사용하는 투사체 삭제 함수
    public int RemoveProjectile(int id)
    {
        int requsetNumber = 0;
        if (Client.Instance.ClientId == -1)
        {
            SingleRemoveProjectile(id);
        }
        else
        {
            requsetNumber = RequestRemoveProjectile(id);
        }

        return requsetNumber;
    }

    // 멀티 일 떄 투사체 삭제를 서버에게 요청합니다.
    private int RequestRemoveProjectile(int id)
    {
        int requestNumber = Random.Range(100, 3000);

        Client.Instance.SendRequestRemoveProjectile(id, requestNumber);

        return requestNumber;
    }

    // 싱글 일 떄 투사체 삭제
    void SingleRemoveProjectile(int id)
    {
        Projectile projectile = null;

        _projectileDictioanry.TryGetValue(id, out projectile);

        if (projectile != null)
        {
            Destroy(projectile.gameObject);
        }
        _projectileDictioanry.Remove(id);
    }

    public void RemoveProjectileByPakcet(S_BroadcastRemoveProjectile packet)
    {
        if (packet == null) return;

        Projectile projectile = null;

        _projectileDictioanry.TryGetValue(packet.projectileId, out projectile);

        if (projectile != null)
        {
            Destroy(projectile.gameObject);
        }
        _projectileDictioanry.Remove(packet.projectileId);
    }


    public void GenerateProjectileByPacket(S_BroadcastGenerateProjectile packet)
    {
        if(packet == null) return;

        int requestNumber = packet.requestNumber;

        Vector3 pos = new Vector3(packet.posX, packet.posY);

        Projectile projectileOrigin = Manager.Data.GetProjectile((ProjectileName)packet.projectileName);

        if (projectileOrigin == null) return;

        Projectile projectile = Instantiate(projectileOrigin.gameObject).GetComponent<Projectile>();
        projectile.ProjectileId = packet.projectileId;
        _projectileDictioanry.Add(projectile.ProjectileId, projectile);

        projectile.transform.position = pos;
        Vector3 fireDirection = new Vector3(packet.fireDirectionX, packet.fireDirectionY);
        projectile.Fire(fireDirection, (CharacterTag)packet.characterTag1, (CharacterTag)packet.characterTag2);
    }


}
