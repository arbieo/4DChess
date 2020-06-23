using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class ChessAI 
{
    ChessBoard originalBoard;

    public const float ADVANCED_PAWN_MULT = 0.15f;
    public const float ADVANCED_UNIT_MULT = 0.1f;
    public const float ADVANCED_PAWN_MIN = 2;
    public const float BOARD_EDGE_DETRIMENT = 0.05f;
    public const float UNCONNECTED_MULT = 0.1f;
    public const float THREATENED_MULT = 0.25f;
    public const float ATTACK_BALANCE_THREATENED_MULT = 0.5f;
    public const float KING_CHECKED_SCORE = 2;
    public const float KING_UNCONTESTED_CHECKED_SCORE = 5;
    public const float OUR_KING_CHECKED_SCORE = 1000;
    public const float KING_FLIGHT_COVERED_SCORE = 2f;
    public const float KING_NO_FLIGHT_SCORE = 5;

    ChessboardAttackerHelper attackHelper;

    public ChessAI(ChessBoard board)
    {
        this.originalBoard = board;
    }

    public ChessBoard.Move DoTurn()
    {
        scoresComputed = 0;
        ChessBoard scratchBoard = originalBoard.DeepCopy();
        attackHelper = new ChessboardAttackerHelper(scratchBoard);

        Dictionary<ChessBoard.Move, float> moveScores = ComputeMoveScores(scratchBoard, 2);
        
        List<ChessBoard.Move> topMoves = new List<ChessBoard.Move>();
        foreach (ChessBoard.Move subMove in moveScores.Keys)
        {
            for (int i = 0; i < 10; i++)
            {
                if (i >= topMoves.Count)
                {
                    topMoves.Add(subMove);
                    break;
                }
                else if(moveScores[subMove] > moveScores[topMoves[i]])
                {
                    topMoves.Insert(i, subMove);
                    break;
                }
            }
        }

        ChessBoard.Move randomGoodMove = topMoves[Random.Range(0, 10)];

        /*if (topMoves[0].pieceCaptured != null || moveScores[topMoves[0]] - moveScores[randomGoodMove] > 5)
        {
            //If the best move is a capture, or is so much better, do it.
            return topMoves[0];
        }*/
        return topMoves[0];
    }

    public int scoresComputed = 0;

    public Dictionary<ChessBoard.Move, float> ComputeMoveScores(ChessBoard board, int movesRemaining)
    {
        List<ChessBoard.Move> possibleMoves = new List<ChessBoard.Move>();
        foreach (ChessPiece piece in board.pieces)
        {
            if (piece == null) continue;
            if (piece.team == board.currentMove)
            {
                foreach (Point4 move in piece.GetValidMoves())
                {
                    possibleMoves.Add(board.GetMove(piece.currentPosition, move));
                }
            }
        }
        Dictionary<ChessBoard.Move, float> moveScores = new Dictionary<ChessBoard.Move, float>();
        foreach (ChessBoard.Move move in possibleMoves)
        {
            board.MovePiece(move);
            float score = -1000000;
            if (movesRemaining <= 1)
            {
                score = EvaluateBoard(board);
            }
            else
            {
               Dictionary<ChessBoard.Move, float> subMoveScores = ComputeMoveScores(board, movesRemaining - 1);
               foreach (KeyValuePair<ChessBoard.Move, float> subMove in subMoveScores)
               {
                   if(subMove.Value > score)
                   {
                       score = subMove.Value;
                   }
               }
               
               score = -score;
            }
            moveScores.Add(move, score);
            board.Undo();
        }

        return moveScores;
    }

    public bool isStepResultReady = false;
    ChessBoard.Move AIStepResultMove;
    Dictionary<ChessBoard.Move, float> AIStepMoveScores = new Dictionary<ChessBoard.Move, float>();
    List<ChessBoard.Move> AIStepPossibleMoves = new List<ChessBoard.Move>();
    ChessBoard AIStepScratchBoard;
    ChessboardAttackerHelper AIStepAttackHelper;


    public ChessBoard.Move ConsumeAIResult()
    {
        isStepResultReady = false;
        List<ChessBoard.Move> topMoves = new List<ChessBoard.Move>();
        foreach (ChessBoard.Move subMove in AIStepMoveScores.Keys)
        {
            for (int i = 0; i < 10; i++)
            {
                if (i >= topMoves.Count)
                {
                    topMoves.Add(subMove);
                    break;
                }
                else if(AIStepMoveScores[subMove] > AIStepMoveScores[topMoves[i]])
                {
                    topMoves.Insert(i, subMove);
                    break;
                }
            }
        }

        ChessBoard.Move randomGoodMove = topMoves[Random.Range(0, 10)];

        /*if (topMoves[0].pieceCaptured != null || moveScores[topMoves[0]] - moveScores[randomGoodMove] > 5)
        {
            //If the best move is a capture, or is so much better, do it.
            return topMoves[0];
        }*/
        Debug.Log("returning move with score: " + AIStepMoveScores[topMoves[0]]);
        return topMoves[0];
    }

    public bool RunAIStep()
    {
        if (isStepResultReady)
        {
            return true;
        }

        if (AIStepPossibleMoves.Count == 0)
        {
            scoresComputed = 0;
            AIStepScratchBoard = originalBoard.DeepCopy();
            attackHelper = new ChessboardAttackerHelper(AIStepScratchBoard);

            AIStepPossibleMoves = new List<ChessBoard.Move>();
            foreach (ChessPiece piece in AIStepScratchBoard.pieces)
            {
                if (piece == null) continue;
                if (piece.team == AIStepScratchBoard.currentMove)
                {
                    foreach (Point4 possibleMove in piece.GetValidMoves())
                    {
                        AIStepPossibleMoves.Add(AIStepScratchBoard.GetMove(piece.currentPosition, possibleMove));
                    }
                }
            }
        }

        ChessBoard.Move move = AIStepPossibleMoves[0];
        AIStepPossibleMoves.RemoveAt(0);
        AIStepScratchBoard.MovePiece(move);
        float score = -1000000;
        Dictionary<ChessBoard.Move, float> subMoveScores = ComputeMoveScores(AIStepScratchBoard, 1);
        foreach (KeyValuePair<ChessBoard.Move, float> subMove in subMoveScores)
        {
            if(subMove.Value > score)
            {
                score = subMove.Value;
            }
        }
            
        score = -score;
        AIStepMoveScores.Add(move, score);
        AIStepScratchBoard.Undo();

        if (AIStepPossibleMoves.Count == 0)
        {
            isStepResultReady = true;
            return true;
        }

        return false;
    }

    public float GetUnitValue(ChessPiece piece)
    {
        if (piece.type == ChessPiece.Type.PAWN)
        {
            return 1;
        }
        else if (piece.type == ChessPiece.Type.ROOK)
        {
            return 3;
        }
        else if (piece.type == ChessPiece.Type.BISHOP)
        {
            return 5;
        }
        else if (piece.type == ChessPiece.Type.KNIGHT)
        {
            return 5;
        }
        else if (piece.type == ChessPiece.Type.QUEEN)
        {
            return 9;
        }
        else
        {
            //We do not include the king in piece value calculations
            return 0;
        }
    }

    public float EvaluateBoard(ChessBoard board)
    {
        ChessPiece.Team enemyTeam = board.currentMove;
        scoresComputed ++;
        
        float enemyScore = 0;
        float alliedScore = 0;

        attackHelper.ComputeAttackers();

        foreach (ChessPiece piece in board.pieces)
        {
            if (piece == null)
            {
                continue;
            }

            if (piece.type == ChessPiece.Type.KING)
            {
                bool inCheck = false;
                ChessPiece checkingPiece = null;
                foreach (ChessPiece attacker in attackHelper.GetAttackers(piece.currentPosition))
                {
                    if (attacker.team != piece.team)
                    {
                        inCheck = true;
                        checkingPiece = piece;
                    }
                }

                bool uncontestedCheck = false;
                if (checkingPiece != null)
                {
                    foreach (ChessPiece attacker in attackHelper.GetAttackers(checkingPiece.currentPosition))
                    {
                        if (attacker.team != checkingPiece.team)
                        {
                            uncontestedCheck = true;
                        }
                    }
                }

                int flightSquareCount = 0;
                int flightSquareTotal = 0;
                foreach (Point4 move in piece.GetValidMoves())
                {
                    bool tileAttacked = false;
                    //Check king flight square and -1 point per square attacked
                    foreach (ChessPiece attacker in attackHelper.GetAttackers(move))
                    {
                        if (attacker.team != piece.team)
                        {
                            tileAttacked = true;
                            break;
                        }
                    }

                    if (!tileAttacked)
                    {
                        flightSquareCount ++;
                    }
                    flightSquareTotal++;
                }

                int flightSquaresCovered = flightSquareTotal - flightSquareCount;

                if (inCheck && board.currentMove != piece.team)
                {
                    return 1000000;
                }
                else
                {
                    // Add/remove score depending on the king position
                    if(piece.team == enemyTeam)
                    {
                        enemyScore += 1000000;
                        enemyScore -= inCheck ? KING_CHECKED_SCORE : 0;
                        enemyScore -= flightSquaresCovered * KING_FLIGHT_COVERED_SCORE;
                        enemyScore -= uncontestedCheck ? KING_UNCONTESTED_CHECKED_SCORE : 0;
                        if (flightSquareCount == 0)
                        {
                            enemyScore -= KING_NO_FLIGHT_SCORE;
                        }
                    }

                    if(piece.team != enemyTeam)
                    {
                        alliedScore += 1000000;
                        alliedScore -= inCheck ? OUR_KING_CHECKED_SCORE : 0;
                        alliedScore -= flightSquaresCovered * KING_FLIGHT_COVERED_SCORE;
                        alliedScore -= uncontestedCheck ? KING_UNCONTESTED_CHECKED_SCORE : 0;
                        if (flightSquareCount == 0)
                        {
                            alliedScore -= KING_NO_FLIGHT_SCORE;
                        }
                    }
                }

                continue;
            }

            float pieceScore = GetUnitValue(piece);
            float teamScore = pieceScore;

            List<ChessPiece> attackers = attackHelper.GetAttackers(piece.currentPosition);

            int friendlyAttacks = 0;
            int enemyAttacks = 0;
            foreach (ChessPiece attacker in attackers)
            {
                if (attacker.team == piece.team)
                {
                    friendlyAttacks ++;
                }
                else if (attacker.team != piece.team)
                {
                    enemyAttacks ++;
                }
            }

            if (friendlyAttacks == 0)
            {
                teamScore -= pieceScore * UNCONNECTED_MULT;
            }
            if (enemyAttacks >= 1)
            {
                if (friendlyAttacks < enemyAttacks)
                {
                    teamScore -= pieceScore * ATTACK_BALANCE_THREATENED_MULT;
                }
                else
                {
                    teamScore -= pieceScore * THREATENED_MULT;
                }
            }

            if (piece.type == ChessPiece.Type.PAWN)
            {
                //Points for pawn push
                int pushAmount = 0;
                if (piece.forwardW.w > 0)
                {
                    pushAmount += piece.w;
                }
                else
                {
                    pushAmount += board.size.w - 1 - piece.w;
                }

                if (piece.forwardZ.z > 0)
                {
                    pushAmount += piece.z;
                }
                else
                {
                    pushAmount += board.size.z - 1 - piece.z;
                }

                pushAmount = (int)Mathf.Max(pushAmount - ADVANCED_PAWN_MIN, 0);
                teamScore += pushAmount * ADVANCED_PAWN_MULT;
            }
            else
            {
                //Points for pawn push
                int pushAmount = 0;
                if (piece.forwardW.w > 0)
                {
                    pushAmount += piece.w;
                }
                else
                {
                    pushAmount += board.size.w - 1 - piece.w;
                }

                teamScore += pushAmount * ADVANCED_UNIT_MULT;
            }

            //Lower score if on the edge of the board
            if(piece.currentPosition.x == 0 || piece.currentPosition.x == board.size.x-1)
            {
                teamScore -= pieceScore * BOARD_EDGE_DETRIMENT;
            }
            if(piece.currentPosition.y == 0 || piece.currentPosition.y == board.size.y-1)
            {
                teamScore -= pieceScore * BOARD_EDGE_DETRIMENT;
            }
            if(piece.currentPosition.z == 0 || piece.currentPosition.z == board.size.z-1)
            {
                teamScore -= pieceScore * BOARD_EDGE_DETRIMENT;
            }
            if(piece.currentPosition.w == 0 || piece.currentPosition.w == board.size.w-1)
            {
                teamScore -= pieceScore * BOARD_EDGE_DETRIMENT;
            }

            if (piece.team == enemyTeam)
            {
                enemyScore += teamScore;
            }
            else
            {
                alliedScore += teamScore;
            }
        }
        
        return alliedScore - enemyScore + Random.Range(-0.1f, 0.1f);
    }
}