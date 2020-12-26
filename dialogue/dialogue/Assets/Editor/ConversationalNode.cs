using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PrimalEmotions
{
      Seeking
    , Rage
    , Fear
    , Lust
    , Care
    , Panic
    , Play
}

public enum AffectiveFeelings
{
      Enthusiastic
    , PissedOff
    , Anxious
    , Horny
    , TenderAndLoving
    , LonelyAndSad
    , Joyous
}


public class ConversationalNode : MonoBehaviour
{
    public string nodeName;
    public int nodeID;
    public string nodeDesc;
    public Vector3 v;


}
