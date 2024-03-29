using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using ChessLogic;
using TesterMethods;

public class Perft
{
    // A Test behaves as an ordinary method
    [Test]
    public void InitialPosition() {

        string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        CheckResult(fen, new int[]{ 20, 400, 8902, 197281, 4865609 });
    }

    [Test]
    public void Position2() {

        string fen = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1";
        CheckResult(fen, new int[]{ 48, 2039, 97862, 4085603 });
    }

    [Test]
    public void Position3() {

        string fen = "8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1";
        CheckResult(fen, new int[]{ 14, 191, 2812, 43238, 674624, 11030083 });
    }

    [Test]
    public void Position4() {

        string fen = "r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1";
        CheckResult(fen, new int[]{ 6, 264, 9467, 422333, 15833292 });
        // CheckSingleResult("r3k2r/Pppp1ppp/1b3nbN/nP6/BBPPP3/q4N2/Pp4PP/R2Q1RK1 b kq - 0 1", 2, 1643);
    }

    [Test]
    public void Position4_5() {

        string fen = "r2q1rk1/pP1p2pp/Q4n2/bbp1p3/Np6/1B3NBn/pPPP1PPP/R3K2R b KQ - 0 1";
        CheckResult(fen, new int[]{ 6, 264, 9467, 422333, 15833292 });
    }

    [Test]
    public void Position5() {

        string fen = "rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8";
        CheckResult(fen, new int[]{ 44, 1486, 62379, 2103487 });
    }

    [Test]
    public void Position6() {

        string fen = "r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10";
        CheckResult(fen, new int[]{ 46, 2079, 89890, 3894594 });
    }

    private void CheckResult(string fen, int[] results) {
        
        for (int i = 0; i < results.Length; i++) {
            Assert.AreEqual(results[i], PositionsCounter.Total(fen, i + 1));
        }
    }

    private void CheckSingleResult(string fen, int depth, int result) {
        Assert.AreEqual(result, PositionsCounter.Total(fen, depth));
    }
}
