﻿using System;
using System.Text;

namespace Horker.DataAnalysis
{
    public class Matrix
    {
        private double[,] _values;

        public int RowCount => _values.GetLength(0);
        public int ColumnCount => _values.GetLength(1);
        public double[,] Values => _values;

        public double this[int row, int column]
        {
            get => _values[row, column];
            set
            {
                _values[row, column] = value;
            }
        }

        public Matrix(int row, int column)
        {
            _values = new double[row, column];
        }

        public Matrix(double[,] values)
        {
            _values = new double[values.GetLength(0), values.GetLength(1)];
            Array.Copy(values, _values, _values.GetLength(0) * _values.GetLength(1));
        }

        public static Matrix Create(double[][] jagged)
        {
            int rowCount = jagged.Length;

            int columnCount = 0;
            foreach (var j in jagged) {
                if (columnCount < j.Length) {
                    columnCount = j.Length;
                }
            }

            var matrix = new Matrix(rowCount, columnCount);

            for (var row = 0; row < rowCount; ++row) {
                for (var column = 0; column < jagged[row].Length; ++column) {
                    matrix[row, column] = jagged[row][column];
                }
            }
            return matrix;
        }

        public static Matrix Create(double[] array, int rowCount = int.MaxValue, int columnCount = int.MaxValue, bool transpose = false)
        {
            if (array.Length == 0) {
                array = new double[1];
            }

            if (columnCount == int.MaxValue) {
                if (rowCount == int.MaxValue) {
                    columnCount = 1;
                    rowCount = array.Length;
                }
                else {
                    columnCount = array.Length / rowCount;
                    if (columnCount == 0 || array.Length % rowCount != 0) {
                        ++columnCount;
                    }
                }
            }
            else {
                if (rowCount == int.MaxValue) {
                    rowCount = array.Length / columnCount;
                    if (rowCount == 0 || array.Length % columnCount != 0) {
                        ++rowCount;
                    }
                }
            }

            var matrix = new Matrix(rowCount, columnCount);

            for (int column = 0; column < columnCount; ++column) {
                for (int row = 0; row < rowCount; ++row) {
                    int index;
                    if (transpose) {
                        index = (row * columnCount + column) % array.Length;
                    }
                    else {
                        index = (column * rowCount + row) % array.Length;
                    }
                    matrix[row, column] = array[index];
                }
            }

            return matrix;
        }

        public static Matrix Diagonal(double value, int rowCount, int columnCount = int.MaxValue)
        {
            if (rowCount == int.MaxValue) {
                rowCount = 1;
            }

            if (columnCount == int.MaxValue) {
                columnCount = rowCount;
            }

            var matrix = new Matrix(rowCount, columnCount);

            var minCount = Math.Min(rowCount, columnCount);
            for (var i = 0; i < minCount; ++i) {
                matrix[i, i] = value;
            }

            return matrix;
        }

        public static Matrix Identity(int rowCount, int columnCount = int.MaxValue)
        {
            return Diagonal(1.0, rowCount, columnCount);
        }

        public static Matrix WithValue(double value, int rowCount, int columnCount = int.MaxValue)
        {
            if (rowCount == int.MaxValue) {
                rowCount = 1;
            }

            if (columnCount == int.MaxValue) {
                columnCount = rowCount;
            }

            var matrix = new Matrix(rowCount, columnCount);
            for (var column = 0; column < columnCount; ++column) {
                for (var row = 0; row < rowCount; ++row) {
                    matrix[row, column] = value;
                }
            }

            return matrix;
        }

        public Matrix Clone()
        {
            return new Matrix(_values);
        }

        public override string ToString()
        {
            string[,] elements = new string[RowCount, ColumnCount];
            int maxLength = int.MinValue;

            for (var column = 0; column < ColumnCount; ++column) {
                for (var row = 0; row < RowCount; ++row) {
                    var s = this[row, column].ToString("0.#####");
                    elements[row, column] = s;
                    if (maxLength < s.Length) {
                        maxLength = s.Length;
                    }
                }
            }

            var builder = new StringBuilder();
            builder.AppendFormat("[{0} x {1}]\r\n", RowCount, ColumnCount);

            for (var row = 0; row < RowCount; ++row) {
                for (var column = 0; column < ColumnCount; ++column) {
                    builder.Append(elements[row, column].PadLeft(maxLength + 1));
                }
                builder.Append("\r\n");
            }

            return builder.ToString();
        }

        public static implicit operator double[,](Matrix matrix)
        {
            return matrix.Values;
        }

        public static implicit operator Matrix(double[,] values)
        {
            return new Matrix(values);
        }

        public static implicit operator Matrix(double[][] jagged)
        {
            return Matrix.Create(jagged);
        }
    }
}