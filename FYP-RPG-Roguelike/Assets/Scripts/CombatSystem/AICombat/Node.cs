using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace combatAI
{
    public class minmaxNode
    {
        public int charHp;
        public int tarHp;
        public CharTeam whoseTurn;
        public int nodeValue;
        public CharState state;
        public List<minmaxNode> nextMoves = new List<minmaxNode>();
        public minmaxNode prevMove;

        public minmaxNode(CharTeam turn, int val)
        {
            whoseTurn = turn;
            nodeValue = val;
        }
    }
}
