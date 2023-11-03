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
	S_EnterSyncInfos = 7,
	C_CharacterInfo = 8,
	S_BroadcastCharacterInfo = 9,
	C_Damage = 10,
	S_BroadcastDamage = 11,
	C_RequestGenerateItem = 12,
	S_BroadcastGenerateItem = 13,
	C_RequestRemoveItem = 14,
	S_BroadcastRemoveItem = 15,
	C_RequestGenerateCharacter = 16,
	S_BroadcastGenerateCharacter = 17,
	C_RequestRemoveCharacter = 18,
	S_BroadcastRemoveCharacter = 19,
	C_RequestGenerateBuilding = 20,
	S_BroadcastGenerateBuilding = 21,
	
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

public class S_EnterSyncInfos : IPacket
{
    public class CharacterInfo
	{
	    public int characterId;
		public int characterName;
		public int hp;
		public float posX;
		public float posY;
		public float posZ;
		public float xSpeed;
		public float ySpeed;
		public int characterState;
		public float characterMoveDirection;
		public int attackType;
		public bool isAttacking;
		public bool isJumping;
		public bool isContactGround;
		public bool isConnectCombo;
		public string etcData;
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
			
			this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			
			this.xSpeed = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			
			this.ySpeed = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			
			this.characterState = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			
			this.characterMoveDirection = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			
			this.attackType = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			
			this.isAttacking = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
			count += sizeof(bool);
			
			this.isJumping = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
			count += sizeof(bool);
			
			this.isContactGround = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
			count += sizeof(bool);
			
			this.isConnectCombo = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
			count += sizeof(bool);
			
			ushort etcDataLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.etcData = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, etcDataLen);
			count += etcDataLen;
			
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
			
			Array.Copy(BitConverter.GetBytes(posZ), 0,segment.Array ,segment.Offset + count, sizeof(float));
			count += sizeof(float);
			
			Array.Copy(BitConverter.GetBytes(xSpeed), 0,segment.Array ,segment.Offset + count, sizeof(float));
			count += sizeof(float);
			
			Array.Copy(BitConverter.GetBytes(ySpeed), 0,segment.Array ,segment.Offset + count, sizeof(float));
			count += sizeof(float);
			
			Array.Copy(BitConverter.GetBytes(characterState), 0,segment.Array ,segment.Offset + count, sizeof(int));
			count += sizeof(int);
			
			Array.Copy(BitConverter.GetBytes(characterMoveDirection), 0,segment.Array ,segment.Offset + count, sizeof(float));
			count += sizeof(float);
			
			Array.Copy(BitConverter.GetBytes(attackType), 0,segment.Array ,segment.Offset + count, sizeof(int));
			count += sizeof(int);
			
			Array.Copy(BitConverter.GetBytes(isAttacking), 0,segment.Array ,segment.Offset + count, sizeof(bool));
			count += sizeof(bool);
			
			Array.Copy(BitConverter.GetBytes(isJumping), 0,segment.Array ,segment.Offset + count, sizeof(bool));
			count += sizeof(bool);
			
			Array.Copy(BitConverter.GetBytes(isContactGround), 0,segment.Array ,segment.Offset + count, sizeof(bool));
			count += sizeof(bool);
			
			Array.Copy(BitConverter.GetBytes(isConnectCombo), 0,segment.Array ,segment.Offset + count, sizeof(bool));
			count += sizeof(bool);
			
			ushort etcDataLen = (ushort)Encoding.Unicode.GetBytes(this.etcData,0,etcData.Length,segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(etcDataLen), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += etcDataLen;
			
	        return success;
	    }
	}
	
	public List<CharacterInfo> characterInfos = new List<CharacterInfo>();
	

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
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_CharacterInfo : IPacket
{
    public int characterId;
	public int hp;
	public float posX;
	public float posY;
	public float posZ;
	public float xSpeed;
	public float ySpeed;
	public int characterState;
	public float characterMoveDirection;
	public int attackType;
	public bool isAttacking;
	public bool isJumping;
	public bool isContactGround;
	public bool isConnectCombo;
	public bool isFrontArmrotation;
	public float frontArmAngle;
	public bool isBehindArmrotation;
	public float behindArmAngle;
	public string etcData;

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
		
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.xSpeed = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.ySpeed = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.characterState = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.characterMoveDirection = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.attackType = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.isAttacking = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		
		this.isJumping = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		
		this.isContactGround = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		
		this.isConnectCombo = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		
		this.isFrontArmrotation = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		
		this.frontArmAngle = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.isBehindArmrotation = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		
		this.behindArmAngle = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		ushort etcDataLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.etcData = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, etcDataLen);
		count += etcDataLen;
		
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
		
		Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(posZ), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(xSpeed), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(ySpeed), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(characterState), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(characterMoveDirection), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(attackType), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(isAttacking), 0,segment.Array ,segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		
		Array.Copy(BitConverter.GetBytes(isJumping), 0,segment.Array ,segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		
		Array.Copy(BitConverter.GetBytes(isContactGround), 0,segment.Array ,segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		
		Array.Copy(BitConverter.GetBytes(isConnectCombo), 0,segment.Array ,segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		
		Array.Copy(BitConverter.GetBytes(isFrontArmrotation), 0,segment.Array ,segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		
		Array.Copy(BitConverter.GetBytes(frontArmAngle), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(isBehindArmrotation), 0,segment.Array ,segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		
		Array.Copy(BitConverter.GetBytes(behindArmAngle), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		ushort etcDataLen = (ushort)Encoding.Unicode.GetBytes(this.etcData,0,etcData.Length,segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(etcDataLen), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += etcDataLen;
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_BroadcastCharacterInfo : IPacket
{
    public int characterId;
	public int hp;
	public float posX;
	public float posY;
	public float posZ;
	public float xSpeed;
	public float ySpeed;
	public int characterState;
	public float characterMoveDirection;
	public int attackType;
	public bool isAttacking;
	public bool isJumping;
	public bool isContactGround;
	public bool isConnectCombo;
	public string etcData;

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
		
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.xSpeed = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.ySpeed = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.characterState = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.characterMoveDirection = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.attackType = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.isAttacking = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		
		this.isJumping = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		
		this.isContactGround = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		
		this.isConnectCombo = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		
		ushort etcDataLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.etcData = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, etcDataLen);
		count += etcDataLen;
		
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
		
		Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(posZ), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(xSpeed), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(ySpeed), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(characterState), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(characterMoveDirection), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(attackType), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(isAttacking), 0,segment.Array ,segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		
		Array.Copy(BitConverter.GetBytes(isJumping), 0,segment.Array ,segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		
		Array.Copy(BitConverter.GetBytes(isContactGround), 0,segment.Array ,segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		
		Array.Copy(BitConverter.GetBytes(isConnectCombo), 0,segment.Array ,segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		
		ushort etcDataLen = (ushort)Encoding.Unicode.GetBytes(this.etcData,0,etcData.Length,segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(etcDataLen), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += etcDataLen;
		

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

public class C_RequestGenerateItem : IPacket
{
    public int requestNumber;
	public int itemName;
	public float posX;
	public float posY;
	public float posZ;

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
		
		this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
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
		
		Array.Copy(BitConverter.GetBytes(posZ), 0,segment.Array ,segment.Offset + count, sizeof(float));
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
	public float posZ;

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
		
		this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
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
		
		Array.Copy(BitConverter.GetBytes(posZ), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_RequestRemoveItem : IPacket
{
    public int itemId;
	public int requestNumber;

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
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_BroadcastRemoveItem : IPacket
{
    public int itemId;
	public bool isSuccess;
	public int requestNumber;

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
	public float posZ;

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
		
		this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
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
		
		Array.Copy(BitConverter.GetBytes(posZ), 0,segment.Array ,segment.Offset + count, sizeof(float));
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
	public float posZ;

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
		
		this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
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
		
		Array.Copy(BitConverter.GetBytes(posZ), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_RequestRemoveCharacter : IPacket
{
    public int characterId;

    public ushort Protocol { get { return (ushort)PacketID.C_RequestRemoveCharacter; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
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
        Array.Copy(BitConverter.GetBytes(characterId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_BroadcastRemoveCharacter : IPacket
{
    public int characterId;

    public ushort Protocol { get { return (ushort)PacketID.S_BroadcastRemoveCharacter; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
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
        Array.Copy(BitConverter.GetBytes(characterId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		

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

