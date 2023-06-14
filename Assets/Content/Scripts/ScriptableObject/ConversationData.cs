using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "ConversationData", menuName = "ScriptableObjects/ConversationData", order = 1 )]
public class ConversationData : ScriptableObject
{
    [SerializeField]
    protected List<Dialogue> dialogueLines;
    public List<Dialogue> DialogueLines => dialogueLines;
}
