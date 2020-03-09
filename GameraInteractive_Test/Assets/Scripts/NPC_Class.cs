using UnityEngine;

[CreateAssetMenu(fileName = "NPC class", menuName = "NPC")]
public class NPC_Class : ScriptableObject
{
    // npc class (race, work, ecc..)
    public enum npcType { villager, merchant, standardEnemy }
    public npcType npcClass;

    // waiting time frame on every waypoint
    public float timeOnWaypoint;

    // NPC field of view (in degrees)
    public float npcFieldOfView;

    // NPC speed rotation towards the player after interaction
    public float rotSpeed;

    // NPC time frame between attacks
    public float attackRatio;
}
