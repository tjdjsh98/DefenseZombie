using ServerCore;
using System.Net;
using System.Text;
using System;
using System.Collections.Generic;

public enum PacketID
{
    C_RequestEnterGame = 1,
	S_AnswerEnterGame = 2,
	C_SuccessToEnterServer = 3,
	S_BroadcastNewClient = 4,
	C_LeaveGame = 5,
	S_BroadcastLeaveGame = 6,
	C_PingPacket = 7,
	S_PingPacket = 8,
	S_EnterSyncInfos = 9,
	C_CharacterControlInfo = 10,
	S_BroadcastCharacterControlInfo = 11,
	C_CharacterInfo = 12,
	S_BroadcastCharacterInfo = 13,
	C_ManagerInfo = 14,
	S_BroadcastManagerInfo = 15,
	C_ItemInfo = 16,
	S_BroadcastItemInfo = 17,
	C_BuildingInfo = 18,
	S_BroadcastBuildingInfo = 19,
	C_BuildingControlInfo = 20,
	S_BroadcastBuildingControlInfo = 21,
	C_Damage = 22,
	S_BroadcastDamage = 23,
	C_RequestGenerateCharacter = 24,
	S_BroadcastGenerateCharacter = 25,
	C_RequestRemoveCharacter = 26,
	S_BroadcastRemoveCharacter = 27,
	C_RequestGenerateItem = 28,
	S_BroadcastGenerateItem = 29,
	C_RequestRemoveItem = 30,
	S_BroadcastRemoveItem = 31,
	C_RequestGenerateBuilding = 32,
	S_BroadcastGenerateBuilding = 33,
	C_RequestRemoveBuilding = 34,
	S_BroadcastRemoveBuilding = 35,
	C_RequestGenerateEffect = 36,
	S_BroadcastGenerateEffect = 37,
	C_RequestGenerateProjectile = 38,
	S_BroadcastGenerateProjectile = 39,
	C_RequestRemoveProjectile = 40,
	S_BroadcastRemoveProjectile = 41,
	
}

public interface IPacket
{
	ushort Protocol { get;}
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}
    

public class C_RequestEnterGame : IPacket
{
    

    public ushort Protocol { get { return (ushort)PacketID.C_RequestEnterGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_RequestEnterGame), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_AnswerEnterGame : IPacket
{
    public bool permission;
	public int playerId;

    public ushort Protocol { get { return (ushort)PacketID.S_AnswerEnterGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.permission = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		
		this.playerId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_AnswerEnterGame), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(permission), 0,segment.Array ,segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		
		Array.Copy(BitConverter.GetBytes(playerId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_SuccessToEnterServer : IPacket
{
    

    public ushort Protocol { get { return (ushort)PacketID.C_SuccessToEnterServer; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_SuccessToEnterServer), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_BroadcastNewClient : IPacket
{
    public int clientId;

    public ushort Protocol { get { return (ushort)PacketID.S_BroadcastNewClient; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.clientId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastNewClient), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(clientId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_LeaveGame : IPacket
{
    

    public ushort Protocol { get { return (ushort)PacketID.C_LeaveGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_LeaveGame), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_BroadcastLeaveGame : IPacket
{
    public int playerId;

    public ushort Protocol { get { return (ushort)PacketID.S_BroadcastLeaveGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.playerId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastLeaveGame), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(playerId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_PingPacket : IPacket
{
    public int time;

    public ushort Protocol { get { return (ushort)PacketID.C_PingPacket; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.time = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_PingPacket), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(time), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_PingPacket : IPacket
{
    public int time;

    public ushort Protocol { get { return (ushort)PacketID.S_PingPacket; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.time = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_PingPacket), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(time), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_EnterSyncInfos : IPacket
{
    public class CharacterInfo
	{
	    public int characterId;
		public int characterName;
		public int hp;
		public float posX;
		public float posY;
		public string data1;
		public string data2;
	    public void Read(ArraySegment<byte> segment, ref ushort count)
	    {
	        this.characterId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			
			this.characterName = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			
			this.hp = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			
			this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			
			this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			
			ushort data1Len = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.data1 = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, data1Len);
			count += data1Len;
			
			ushort data2Len = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.data2 = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, data2Len);
			count += data2Len;
			
	    }
	
	    public bool Write(ArraySegment<byte> segment, ref ushort count)
	    {
	        bool success = true;
	        Array.Copy(BitConverter.GetBytes(characterId), 0,segment.Array ,segment.Offset + count, sizeof(int));
			count += sizeof(int);
			
			Array.Copy(BitConverter.GetBytes(characterName), 0,segment.Array ,segment.Offset + count, sizeof(int));
			count += sizeof(int);
			
			Array.Copy(BitConverter.GetBytes(hp), 0,segment.Array ,segment.Offset + count, sizeof(int));
			count += sizeof(int);
			
			Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
			count += sizeof(float);
			
			Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
			count += sizeof(float);
			
			ushort data1Len = (ushort)Encoding.Unicode.GetBytes(this.data1,0,data1.Length,segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(data1Len), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += data1Len;
			
			ushort data2Len = (ushort)Encoding.Unicode.GetBytes(this.data2,0,data2.Length,segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(data2Len), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += data2Len;
			
	        return success;
	    }
	}
	
	public List<CharacterInfo> characterInfos = new List<CharacterInfo>();
	
	public class ItemInfo
	{
	    public int itemId;
		public int itemName;
		public float posX;
		public float posY;
		public string data;
	    public void Read(ArraySegment<byte> segment, ref ushort count)
	    {
	        this.itemId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			
			this.itemName = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			
			this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			
			this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			
			ushort dataLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.data = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, dataLen);
			count += dataLen;
			
	    }
	
	    public bool Write(ArraySegment<byte> segment, ref ushort count)
	    {
	        bool success = true;
	        Array.Copy(BitConverter.GetBytes(itemId), 0,segment.Array ,segment.Offset + count, sizeof(int));
			count += sizeof(int);
			
			Array.Copy(BitConverter.GetBytes(itemName), 0,segment.Array ,segment.Offset + count, sizeof(int));
			count += sizeof(int);
			
			Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
			count += sizeof(float);
			
			Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
			count += sizeof(float);
			
			ushort dataLen = (ushort)Encoding.Unicode.GetBytes(this.data,0,data.Length,segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(dataLen), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += dataLen;
			
	        return success;
	    }
	}
	
	public List<ItemInfo> itemInfos = new List<ItemInfo>();
	
	public class BuildingInfo
	{
	    public int buildingId;
		public int buildingName;
		public int hp;
		public float posX;
		public float posY;
		public int cellPosX;
		public int cellPosY;
		public string data1;
		public string data2;
	    public void Read(ArraySegment<byte> segment, ref ushort count)
	    {
	        this.buildingId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			
			this.buildingName = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			
			this.hp = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			
			this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			
			this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			
			this.cellPosX = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			
			this.cellPosY = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			
			ushort data1Len = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.data1 = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, data1Len);
			count += data1Len;
			
			ushort data2Len = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.data2 = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, data2Len);
			count += data2Len;
			
	    }
	
	    public bool Write(ArraySegment<byte> segment, ref ushort count)
	    {
	        bool success = true;
	        Array.Copy(BitConverter.GetBytes(buildingId), 0,segment.Array ,segment.Offset + count, sizeof(int));
			count += sizeof(int);
			
			Array.Copy(BitConverter.GetBytes(buildingName), 0,segment.Array ,segment.Offset + count, sizeof(int));
			count += sizeof(int);
			
			Array.Copy(BitConverter.GetBytes(hp), 0,segment.Array ,segment.Offset + count, sizeof(int));
			count += sizeof(int);
			
			Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
			count += sizeof(float);
			
			Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
			count += sizeof(float);
			
			Array.Copy(BitConverter.GetBytes(cellPosX), 0,segment.Array ,segment.Offset + count, sizeof(int));
			count += sizeof(int);
			
			Array.Copy(BitConverter.GetBytes(cellPosY), 0,segment.Array ,segment.Offset + count, sizeof(int));
			count += sizeof(int);
			
			ushort data1Len = (ushort)Encoding.Unicode.GetBytes(this.data1,0,data1.Length,segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(data1Len), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += data1Len;
			
			ushort data2Len = (ushort)Encoding.Unicode.GetBytes(this.data2,0,data2.Length,segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(data2Len), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += data2Len;
			
	        return success;
	    }
	}
	
	public List<BuildingInfo> buildingInfos = new List<BuildingInfo>();
	
	public class ProjectileInfo
	{
	    public int projectileId;
		public int projectileName;
		public float posX;
		public float posY;
		public string data;
	    public void Read(ArraySegment<byte> segment, ref ushort count)
	    {
	        this.projectileId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			
			this.projectileName = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			
			this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			
			this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			
			ushort dataLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.data = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, dataLen);
			count += dataLen;
			
	    }
	
	    public bool Write(ArraySegment<byte> segment, ref ushort count)
	    {
	        bool success = true;
	        Array.Copy(BitConverter.GetBytes(projectileId), 0,segment.Array ,segment.Offset + count, sizeof(int));
			count += sizeof(int);
			
			Array.Copy(BitConverter.GetBytes(projectileName), 0,segment.Array ,segment.Offset + count, sizeof(int));
			count += sizeof(int);
			
			Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
			count += sizeof(float);
			
			Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
			count += sizeof(float);
			
			ushort dataLen = (ushort)Encoding.Unicode.GetBytes(this.data,0,data.Length,segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(dataLen), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += dataLen;
			
	        return success;
	    }
	}
	
	public List<ProjectileInfo> projectileInfos = new List<ProjectileInfo>();
	
	public class ManagerInfo
	{
	    public int managerName;
		public string data;
	    public void Read(ArraySegment<byte> segment, ref ushort count)
	    {
	        this.managerName = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			
			ushort dataLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.data = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, dataLen);
			count += dataLen;
			
	    }
	
	    public bool Write(ArraySegment<byte> segment, ref ushort count)
	    {
	        bool success = true;
	        Array.Copy(BitConverter.GetBytes(managerName), 0,segment.Array ,segment.Offset + count, sizeof(int));
			count += sizeof(int);
			
			ushort dataLen = (ushort)Encoding.Unicode.GetBytes(this.data,0,data.Length,segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(dataLen), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += dataLen;
			
	        return success;
	    }
	}
	
	public List<ManagerInfo> managerInfos = new List<ManagerInfo>();
	

    public ushort Protocol { get { return (ushort)PacketID.S_EnterSyncInfos; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.characterInfos.Clear();
		ushort characterInfoLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		for(int i =0; i < characterInfoLen; i++)
		{
		    CharacterInfo characterInfo = new CharacterInfo();
		    characterInfo.Read(segment, ref count);
		    characterInfos.Add(characterInfo);
		}
		
		this.itemInfos.Clear();
		ushort itemInfoLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		for(int i =0; i < itemInfoLen; i++)
		{
		    ItemInfo itemInfo = new ItemInfo();
		    itemInfo.Read(segment, ref count);
		    itemInfos.Add(itemInfo);
		}
		
		this.buildingInfos.Clear();
		ushort buildingInfoLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		for(int i =0; i < buildingInfoLen; i++)
		{
		    BuildingInfo buildingInfo = new BuildingInfo();
		    buildingInfo.Read(segment, ref count);
		    buildingInfos.Add(buildingInfo);
		}
		
		this.projectileInfos.Clear();
		ushort projectileInfoLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		for(int i =0; i < projectileInfoLen; i++)
		{
		    ProjectileInfo projectileInfo = new ProjectileInfo();
		    projectileInfo.Read(segment, ref count);
		    projectileInfos.Add(projectileInfo);
		}
		
		this.managerInfos.Clear();
		ushort managerInfoLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		for(int i =0; i < managerInfoLen; i++)
		{
		    ManagerInfo managerInfo = new ManagerInfo();
		    managerInfo.Read(segment, ref count);
		    managerInfos.Add(managerInfo);
		}
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_EnterSyncInfos), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)this.characterInfos.Count), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach (CharacterInfo characterInfo in characterInfos)
		    characterInfo.Write(segment,ref count);
		
		Array.Copy(BitConverter.GetBytes((ushort)this.itemInfos.Count), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach (ItemInfo itemInfo in itemInfos)
		    itemInfo.Write(segment,ref count);
		
		Array.Copy(BitConverter.GetBytes((ushort)this.buildingInfos.Count), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach (BuildingInfo buildingInfo in buildingInfos)
		    buildingInfo.Write(segment,ref count);
		
		Array.Copy(BitConverter.GetBytes((ushort)this.projectileInfos.Count), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach (ProjectileInfo projectileInfo in projectileInfos)
		    projectileInfo.Write(segment,ref count);
		
		Array.Copy(BitConverter.GetBytes((ushort)this.managerInfos.Count), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach (ManagerInfo managerInfo in managerInfos)
		    managerInfo.Write(segment,ref count);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_CharacterControlInfo : IPacket
{
    public int characterId;
	public float posX;
	public float posY;
	public string data;

    public ushort Protocol { get { return (ushort)PacketID.C_CharacterControlInfo; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.characterId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		ushort dataLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.data = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, dataLen);
		count += dataLen;
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_CharacterControlInfo), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(characterId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		ushort dataLen = (ushort)Encoding.Unicode.GetBytes(this.data,0,data.Length,segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(dataLen), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += dataLen;
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_BroadcastCharacterControlInfo : IPacket
{
    public int characterId;
	public float posX;
	public float posY;
	public string data;

    public ushort Protocol { get { return (ushort)PacketID.S_BroadcastCharacterControlInfo; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.characterId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		ushort dataLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.data = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, dataLen);
		count += dataLen;
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastCharacterControlInfo), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(characterId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		ushort dataLen = (ushort)Encoding.Unicode.GetBytes(this.data,0,data.Length,segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(dataLen), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += dataLen;
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_CharacterInfo : IPacket
{
    public int characterId;
	public int hp;
	public string data;

    public ushort Protocol { get { return (ushort)PacketID.C_CharacterInfo; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.characterId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.hp = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		ushort dataLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.data = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, dataLen);
		count += dataLen;
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_CharacterInfo), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(characterId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(hp), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		ushort dataLen = (ushort)Encoding.Unicode.GetBytes(this.data,0,data.Length,segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(dataLen), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += dataLen;
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_BroadcastCharacterInfo : IPacket
{
    public int characterId;
	public int hp;
	public string data;

    public ushort Protocol { get { return (ushort)PacketID.S_BroadcastCharacterInfo; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.characterId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.hp = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		ushort dataLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.data = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, dataLen);
		count += dataLen;
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastCharacterInfo), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(characterId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(hp), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		ushort dataLen = (ushort)Encoding.Unicode.GetBytes(this.data,0,data.Length,segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(dataLen), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += dataLen;
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_ManagerInfo : IPacket
{
    public int managerName;
	public string data;

    public ushort Protocol { get { return (ushort)PacketID.C_ManagerInfo; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.managerName = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		ushort dataLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.data = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, dataLen);
		count += dataLen;
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_ManagerInfo), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(managerName), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		ushort dataLen = (ushort)Encoding.Unicode.GetBytes(this.data,0,data.Length,segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(dataLen), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += dataLen;
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_BroadcastManagerInfo : IPacket
{
    public int managerName;
	public string data;

    public ushort Protocol { get { return (ushort)PacketID.S_BroadcastManagerInfo; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.managerName = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		ushort dataLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.data = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, dataLen);
		count += dataLen;
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastManagerInfo), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(managerName), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		ushort dataLen = (ushort)Encoding.Unicode.GetBytes(this.data,0,data.Length,segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(dataLen), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += dataLen;
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_ItemInfo : IPacket
{
    public int itemId;
	public float posX;
	public float posY;
	public string data;

    public ushort Protocol { get { return (ushort)PacketID.C_ItemInfo; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.itemId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		ushort dataLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.data = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, dataLen);
		count += dataLen;
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_ItemInfo), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(itemId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		ushort dataLen = (ushort)Encoding.Unicode.GetBytes(this.data,0,data.Length,segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(dataLen), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += dataLen;
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_BroadcastItemInfo : IPacket
{
    public int itemId;
	public float posX;
	public float posY;
	public string data;

    public ushort Protocol { get { return (ushort)PacketID.S_BroadcastItemInfo; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.itemId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		ushort dataLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.data = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, dataLen);
		count += dataLen;
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastItemInfo), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(itemId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		ushort dataLen = (ushort)Encoding.Unicode.GetBytes(this.data,0,data.Length,segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(dataLen), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += dataLen;
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_BuildingInfo : IPacket
{
    public int buildingId;
	public int hp;
	public float posX;
	public float posY;
	public int cellPosX;
	public int cellPosY;
	public string data;

    public ushort Protocol { get { return (ushort)PacketID.C_BuildingInfo; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.buildingId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.hp = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.cellPosX = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.cellPosY = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		ushort dataLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.data = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, dataLen);
		count += dataLen;
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_BuildingInfo), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(buildingId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(hp), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(cellPosX), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(cellPosY), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		ushort dataLen = (ushort)Encoding.Unicode.GetBytes(this.data,0,data.Length,segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(dataLen), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += dataLen;
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_BroadcastBuildingInfo : IPacket
{
    public int buildingId;
	public int hp;
	public float posX;
	public float posY;
	public int cellPosX;
	public int cellPosY;
	public string data;

    public ushort Protocol { get { return (ushort)PacketID.S_BroadcastBuildingInfo; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.buildingId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.hp = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.cellPosX = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.cellPosY = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		ushort dataLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.data = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, dataLen);
		count += dataLen;
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastBuildingInfo), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(buildingId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(hp), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(cellPosX), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(cellPosY), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		ushort dataLen = (ushort)Encoding.Unicode.GetBytes(this.data,0,data.Length,segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(dataLen), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += dataLen;
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_BuildingControlInfo : IPacket
{
    public int buildingId;
	public string data;

    public ushort Protocol { get { return (ushort)PacketID.C_BuildingControlInfo; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.buildingId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		ushort dataLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.data = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, dataLen);
		count += dataLen;
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_BuildingControlInfo), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(buildingId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		ushort dataLen = (ushort)Encoding.Unicode.GetBytes(this.data,0,data.Length,segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(dataLen), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += dataLen;
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_BroadcastBuildingControlInfo : IPacket
{
    public int buildingId;
	public string data;

    public ushort Protocol { get { return (ushort)PacketID.S_BroadcastBuildingControlInfo; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.buildingId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		ushort dataLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.data = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, dataLen);
		count += dataLen;
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastBuildingControlInfo), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(buildingId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		ushort dataLen = (ushort)Encoding.Unicode.GetBytes(this.data,0,data.Length,segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(dataLen), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += dataLen;
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_Damage : IPacket
{
    public int characterId;
	public float directionX;
	public float directionY;
	public float power;
	public float stagger;

    public ushort Protocol { get { return (ushort)PacketID.C_Damage; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.characterId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.directionX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.directionY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.power = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.stagger = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_Damage), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(characterId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(directionX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(directionY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(power), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(stagger), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_BroadcastDamage : IPacket
{
    public int characterId;
	public float directionX;
	public float directionY;
	public float power;
	public float stagger;

    public ushort Protocol { get { return (ushort)PacketID.S_BroadcastDamage; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.characterId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.directionX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.directionY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.power = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.stagger = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastDamage), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(characterId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(directionX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(directionY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(power), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(stagger), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_RequestGenerateCharacter : IPacket
{
    public int requestNumber;
	public bool isPlayerableChracter;
	public int characterName;
	public float posX;
	public float posY;

    public ushort Protocol { get { return (ushort)PacketID.C_RequestGenerateCharacter; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.requestNumber = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.isPlayerableChracter = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		
		this.characterName = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_RequestGenerateCharacter), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(requestNumber), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(isPlayerableChracter), 0,segment.Array ,segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		
		Array.Copy(BitConverter.GetBytes(characterName), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_BroadcastGenerateCharacter : IPacket
{
    public int requestNumber;
	public bool isSuccess;
	public int characterId;
	public int characterName;
	public float posX;
	public float posY;

    public ushort Protocol { get { return (ushort)PacketID.S_BroadcastGenerateCharacter; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.requestNumber = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.isSuccess = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		
		this.characterId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.characterName = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastGenerateCharacter), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(requestNumber), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(isSuccess), 0,segment.Array ,segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		
		Array.Copy(BitConverter.GetBytes(characterId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(characterName), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_RequestRemoveCharacter : IPacket
{
    public int requestNumber;
	public int characterId;

    public ushort Protocol { get { return (ushort)PacketID.C_RequestRemoveCharacter; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.requestNumber = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.characterId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_RequestRemoveCharacter), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(requestNumber), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(characterId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_BroadcastRemoveCharacter : IPacket
{
    public int requestNumber;
	public int characterId;

    public ushort Protocol { get { return (ushort)PacketID.S_BroadcastRemoveCharacter; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.requestNumber = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.characterId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastRemoveCharacter), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(requestNumber), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(characterId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_RequestGenerateItem : IPacket
{
    public int requestNumber;
	public int itemName;
	public float posX;
	public float posY;

    public ushort Protocol { get { return (ushort)PacketID.C_RequestGenerateItem; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.requestNumber = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.itemName = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_RequestGenerateItem), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(requestNumber), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(itemName), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_BroadcastGenerateItem : IPacket
{
    public int requestNumber;
	public bool isSuccess;
	public int itemId;
	public int itemName;
	public float posX;
	public float posY;

    public ushort Protocol { get { return (ushort)PacketID.S_BroadcastGenerateItem; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.requestNumber = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.isSuccess = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		
		this.itemId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.itemName = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastGenerateItem), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(requestNumber), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(isSuccess), 0,segment.Array ,segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		
		Array.Copy(BitConverter.GetBytes(itemId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(itemName), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_RequestRemoveItem : IPacket
{
    public int itemId;
	public int requestNumber;
	public float posX;
	public float posY;

    public ushort Protocol { get { return (ushort)PacketID.C_RequestRemoveItem; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.itemId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.requestNumber = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_RequestRemoveItem), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(itemId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(requestNumber), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_BroadcastRemoveItem : IPacket
{
    public int itemId;
	public bool isSuccess;
	public int requestNumber;
	public float posX;
	public float posY;

    public ushort Protocol { get { return (ushort)PacketID.S_BroadcastRemoveItem; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.itemId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.isSuccess = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		
		this.requestNumber = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastRemoveItem), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(itemId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(isSuccess), 0,segment.Array ,segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		
		Array.Copy(BitConverter.GetBytes(requestNumber), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_RequestGenerateBuilding : IPacket
{
    public int requestNumber;
	public int buildingName;
	public int posX;
	public int posY;

    public ushort Protocol { get { return (ushort)PacketID.C_RequestGenerateBuilding; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.requestNumber = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.buildingName = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.posX = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.posY = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_RequestGenerateBuilding), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(requestNumber), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(buildingName), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_BroadcastGenerateBuilding : IPacket
{
    public int requestNumber;
	public int buildingId;
	public int buildingName;
	public bool isSuccess;
	public int posX;
	public int posY;

    public ushort Protocol { get { return (ushort)PacketID.S_BroadcastGenerateBuilding; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.requestNumber = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.buildingId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.buildingName = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.isSuccess = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		
		this.posX = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.posY = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastGenerateBuilding), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(requestNumber), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(buildingId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(buildingName), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(isSuccess), 0,segment.Array ,segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		
		Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_RequestRemoveBuilding : IPacket
{
    public int requestNumber;
	public int buildingId;

    public ushort Protocol { get { return (ushort)PacketID.C_RequestRemoveBuilding; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.requestNumber = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.buildingId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_RequestRemoveBuilding), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(requestNumber), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(buildingId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_BroadcastRemoveBuilding : IPacket
{
    public int requestNumber;
	public int buildingId;

    public ushort Protocol { get { return (ushort)PacketID.S_BroadcastRemoveBuilding; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.requestNumber = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.buildingId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastRemoveBuilding), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(requestNumber), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(buildingId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_RequestGenerateEffect : IPacket
{
    public int requestNumber;
	public int effectName;
	public float posX;
	public float posY;

    public ushort Protocol { get { return (ushort)PacketID.C_RequestGenerateEffect; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.requestNumber = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.effectName = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_RequestGenerateEffect), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(requestNumber), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(effectName), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_BroadcastGenerateEffect : IPacket
{
    public int requestNumber;
	public int effectName;
	public float posX;
	public float posY;

    public ushort Protocol { get { return (ushort)PacketID.S_BroadcastGenerateEffect; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.requestNumber = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.effectName = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastGenerateEffect), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(requestNumber), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(effectName), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_RequestGenerateProjectile : IPacket
{
    public int requestNumber;
	public int projectileName;
	public float posX;
	public float posY;
	public float fireDirectionX;
	public float fireDirectionY;
	public int characterTag1;
	public int characterTag2;
	public int damage;

    public ushort Protocol { get { return (ushort)PacketID.C_RequestGenerateProjectile; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.requestNumber = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.projectileName = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.fireDirectionX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.fireDirectionY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.characterTag1 = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.characterTag2 = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.damage = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_RequestGenerateProjectile), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(requestNumber), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(projectileName), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(fireDirectionX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(fireDirectionY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(characterTag1), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(characterTag2), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(damage), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_BroadcastGenerateProjectile : IPacket
{
    public int requestNumber;
	public int projectileId;
	public int projectileName;
	public bool isSuccess;
	public float posX;
	public float posY;
	public float fireDirectionX;
	public float fireDirectionY;
	public int characterTag1;
	public int characterTag2;
	public int damage;

    public ushort Protocol { get { return (ushort)PacketID.S_BroadcastGenerateProjectile; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.requestNumber = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.projectileId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.projectileName = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.isSuccess = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.fireDirectionX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.fireDirectionY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.characterTag1 = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.characterTag2 = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.damage = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastGenerateProjectile), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(requestNumber), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(projectileId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(projectileName), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(isSuccess), 0,segment.Array ,segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		
		Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(fireDirectionX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(fireDirectionY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(characterTag1), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(characterTag2), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(damage), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_RequestRemoveProjectile : IPacket
{
    public int requestNumber;
	public int projectileId;

    public ushort Protocol { get { return (ushort)PacketID.C_RequestRemoveProjectile; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.requestNumber = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.projectileId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_RequestRemoveProjectile), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(requestNumber), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(projectileId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_BroadcastRemoveProjectile : IPacket
{
    public int requestNumber;
	public int projectileId;

    public ushort Protocol { get { return (ushort)PacketID.S_BroadcastRemoveProjectile; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.requestNumber = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.projectileId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastRemoveProjectile), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(requestNumber), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(projectileId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

