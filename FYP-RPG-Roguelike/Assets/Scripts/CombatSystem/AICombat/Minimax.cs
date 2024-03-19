using combatAI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Minimax
{
    public CombatCharacterController charCont;

    public bool isDoneChoosing = false;
    List<AbilityData> _abs;

    public Minimax(CombatCharacterController cont)
    {
        charCont = cont;
        _abs = new List<AbilityData>();
        _abs = charCont.characterData.charAbilities;
        _abs.Add(charCont.characterData.basicAttack);
    }

    public AbilityData[] getBestMoves()
    {

        AbilityData[] moves = _abs.OrderByDescending(a => a.abilityValue)
                                 .Where(canCast)
                                 .Take(2)
                                 .ToArray();

        return moves;
    }

    public bool canCast(AbilityData _ability)
    {
        if (charCont.characterData.currMana >= _ability.manaCost)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public int calculateNodeCost(CombatCharacterController character)
    {
        int healthDifference = (int)(character.characterData.currHealth - character.characterData.targetData.currHealth);
        AbilityData[] moves = getBestMoves();
        int manaAvg=0;
        int dmgAvg = 0;
        foreach(var move in moves)
        {
            manaAvg += move.manaCost;
            dmgAvg += move.abilityValue;
        }
        manaAvg = (int)-((character.characterData.currMana * manaAvg)) / moves.Length; 
        dmgAvg = dmgAvg / moves.Length;
        int totalScore = (healthDifference-manaAvg+dmgAvg);

        return totalScore;
    }

    public int miniMax(CombatCharacterController character, int depth, bool maximizingPlayer)
    {
        if (depth == 0 || character.characterData.characterState == CharState.Dead || character.characterData.characterState == CharState.Finished)
        {
            return calculateNodeCost(character);
        }

        if (maximizingPlayer)
        {
            int maxEval = int.MinValue;
            AbilityData[] possibleMoves = getBestMoves();

            foreach (AbilityData move in possibleMoves)
            {
                SimulateMove(move);

                int eval = miniMax(character, depth - 1, false);
                maxEval = Mathf.Max(maxEval, eval);

                UndoMove(move);
            }

            return maxEval;
        }
        else
        {
            int minEval = int.MaxValue;
            AbilityData[] possibleMoves = getBestMoves();

            foreach (AbilityData move in possibleMoves)
            {
                SimulateMove(move);

                int eval = miniMax(character, depth - 1, true);
                minEval = Mathf.Min(minEval, eval);

                UndoMove(move);
            }

            return minEval;
        }

    }

    public AbilityData ChooseBestMove()
    {
        int bestScore = int.MinValue;
        AbilityData bestMove = null;
        AbilityData[] possibleMoves = getBestMoves();

        foreach (AbilityData move in possibleMoves)
        {

            SimulateMove(move);

            int score = miniMax(charCont, 2, false);

            UndoMove(move);

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
        }

        isDoneChoosing = true;
        return bestMove;
    }

    public void SimulateMove(AbilityData _move)
    {
        charCont.characterData.targetData.Damage(_move.abilityValue);
    }

    public void UndoMove(AbilityData _move)
    {
        charCont.characterData.targetData.Heal(_move.abilityValue);
    }
}
