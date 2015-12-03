using Edge.Looping;
using Edge.Matrix;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static CoreTest.MatrixAssert;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace CoreTest
{
    public static class MatrixAssert
    {
        public static void Islike(Matrix<double> a, double[,] b)
        {
            AreEqual(a.rows, b.GetLength(0));
            AreEqual(a.collumns, b.GetLength(1));
            foreach (var t in b.CoordinateBind())
                AreEqual(a[t.Item2, t.Item3], b[t.Item2, t.Item3], 0.1);
        }
    }
    [TestClass]
    public class MatrixTest
    {
        [TestMethod] public void Construction()
        {
            var val = Matrix<double>.fromArr(new double[,] {{0, 1, 2}, {3, 4, 5}, {6, 7, 8}});
            foreach (var t in Loops.Range(3).Join())
                AreEqual(val[t.Item1, t.Item2], t.Item1 * 3 + t.Item2);
        }
        [TestMethod] public void Multiplication1()
        {
            var val1 = Matrix<double>.fromArr(new double[,] {{0, 1, 2}, {3, 4, 5}, {6, 7, 8}});
            var val2 = Matrix<double>.fromArr(new double[,] {{3, 2, 1}, {6, 5, 4}, {9, 8, 7}});
            Islike(val1 * val2, new double[,] {{24, 21, 18}, {78, 66, 54}, {132, 111, 90}});
        }
        [TestMethod] public void Multiplication2()
        {
            var val1 = Matrix<double>.fromArr(new double[,] {{0, 1, 2}, {3, 4, 5}, {6, 7, 8}, {9, 10, 11}});
            var val2 = Matrix<double>.fromArr(new double[,] {{3, 2}, {6, 5}, {9, 8}});
            Islike(val1 * val2, new double[,] {{24, 21}, {78, 66}, {132, 111}, {186, 156}});
        }
        [TestMethod]
        public void MultiplicationByScalar()
        {
            var val1 = Matrix<double>.fromArr(new double[,] { { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 }, { 9, 10, 11 } });
            Islike(val1 * 1.2, new double[,] { { 0, 1.2,2.4 }, { 3.6,4.8, 6 }, { 7.2, 8.4, 9.6 }, { 10.8, 12, 13.2 } });
        }
        [TestMethod] public void Square()
        {
            var val1 = Matrix<double>.fromArr(new double[,] {{0, 1, 2}, {3, 4, 5}, {6, 7, 8}});
            IsTrue(val1.isSquare);
            val1 = Matrix<double>.fromArr(new double[,] {{0, 1, 2}, {3, 4, 5}});
            IsFalse(val1.isSquare);
            val1 = Matrix<double>.fromArr(new double[,] {});
            IsTrue(val1.isSquare);
        }
        [TestMethod] public void Vector()
        {
            var val1 = Matrix<double>.fromArr(new double[,] {{0, 1, 2}});
            AreEqual(val1.isVector, Matrix<double>.VectorType.Row);
            val1 = Matrix<double>.fromArr(new double[,] {{0}, {3}});
            AreEqual(val1.isVector, Matrix<double>.VectorType.Collumn);
            val1 = Matrix<double>.fromArr(new double[,] {});
            AreEqual(val1.isVector, Matrix<double>.VectorType.None);
            val1 = Matrix<double>.fromArr(new double[,] {{1, 2}, {3, 4}});
            AreEqual(val1.isVector, Matrix<double>.VectorType.None);
        }
        [TestMethod] public void SubMatrix()
        {
            var val1 = Matrix<double>.fromArr(new double[,] {{0, 1, 2}, {3, 4, 5}, {6, 7, 8}, {9, 10, 11}}).subMatrix(1, 0);
            Islike(val1, new double[,] {{1, 2}, {7, 8}, {10, 11}});
        }
        [TestMethod] public void Cofactors()
        {
            var val = Matrix<double>.fromArr(new double[,] {{1, 2, 3}, {6, 5, 4}, {8, 5, 2}}).Cofactor();
            Islike(val, new double[,] {{-10, 20, -10}, {11, -22, 11}, {-7, 14, -7}});
        }
        [TestMethod] public void Determinant()
        {
            var val = Matrix<double>.fromArr(new double[,] {{1, 2, 3}, {6, 2, 4}, {8, 5, 2}}).determinant();
            AreEqual(val, 66, 0.0001);
            val = Matrix<double>.fromArr(new double[,] {{1, 2, 3}, {6, 5, 4}, {8, 5, 2}}).determinant();
            AreEqual(val, 0, 0.0001);
        }
        [TestMethod] public void Invert()
        {
            var val = Matrix<double>.fromArr(new double[,] {{1, 2, 3}, {6, 2, 4}, {8, 5, 2}}).inverse();
            Islike(val, new double[,] {{-8.0 / 33, 1.0 / 6, 1.0 / 33}, {10.0 / 33, -1.0 / 3, 7.0 / 33}, {7.0 / 33, 1.0 / 6, -5.0 / 33}});
        }
        [TestMethod] public void Transpose()
        {
            var val = Matrix<double>.fromArr(new double[,] {{1, 2, 3}, {6, 2, 4}, {8, 5, 2}}).transpose();
            Islike(val, new double[,] {{1, 6, 8}, {2, 2, 5}, {3, 4, 2}});
        }
        [TestMethod]
        public void Addition1()
        {
            var val1 = Matrix<double>.fromArr(new double[,] { { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 } });
            var val2 = Matrix<double>.fromArr(new double[,] { { 3, 2, 1 }, { 6, 5, 4 }, { 9, 8, 7 } });
            Islike(val1 + val2, new double[,] { { 3,3,3 }, { 9,9,9 }, { 15,15,15 } });
        }
        [TestMethod]
        public void Addition2()
        {
            var val1 = Matrix<double>.fromArr(new double[,] { { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 }, { 9, 10, 11 } });
            var val2 = Matrix<double>.fromArr(new double[,] { {8,3,5 }, { 8,5,4 }, { 4,1,2 }, { 5,2,1 } });
            Islike(val1 + val2, new double[,] { { 8,4,7 }, { 11,9,9 }, { 10,8,10 }, { 14,12,12 } });
        }
        [TestMethod] public void MiscArithmatic()
        {
            var val1 = Matrix<double>.fromArr(new double[,] { { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 }});
            var val2 = Matrix<double>.fromArr(new double[,] { { 8, 3, 5 }, { 8, 5, 4 }, { 4, 1, 2 }});
            var res = (val1 * (Matrix<double>.getIdent() + Matrix<double>.getIdent()) * val2 * (val1 + val2)) + (val1.pow(3));
            Islike(res, new double[,] { { 750, 616, 798 }, { 3342, 2596, 3378 }, { 5934, 4576, 5958 } });
        }
        [TestMethod] public void Exponentiation()
        {
            var val = Matrix<double>.fromArr(new double[,] { { 1, 2, 3 }, { 6, 2, 4 }, { 8, 5, 2 } }).exp(0.001);
            Islike(val, new double[,] { { 8590.33, 5569.56, 5796.94 }, { 14415.8, 9346.83, 9728.13 }, { 16987.1, 11013.7, 11463.3 } });
        }
        [TestMethod] public void Trace()
        {
            var val = Matrix<double>.fromArr(new double[,] { { 1, 2, 3 }, { 6, 2, 4 }, { 8, 5, 2 } }).Trace();
            AreEqual(val,5);
        }
        [TestMethod] public void Dot()
        {
            var val1 = Matrix<double>.fromArr(new double[,] { { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 }});
            var val2 = Matrix<double>.fromArr(new double[,] { { 8, 3, 5 }, { 8, 5, 4 }, { 4, 1, 2 } });
            AreEqual(val2.dotProduct(val1), 124);
        }
    }
}