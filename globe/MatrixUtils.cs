

using System.Runtime.Serialization.Formatters.Binary;

namespace Matrix
{
    public class Matrix<T>
    {
        T _def;
        T[][] _raw_vector;

        int cols;
        int rows;

        public Matrix(int rows, int cols, T default_value)
        {
            T[] base_row= new T[cols];
            for(int i = 0; i < cols; ++i)
                base_row[i] = default_value;

            this._def = default_value; 

            this._raw_vector = new T[rows][];
            for(int i = 0; i < rows; ++i)
                this._raw_vector[i] = base_row;

            this.cols = cols;
            this.rows = rows;

        }

        public Matrix(in Matrix<T> matrix){
            this._def = matrix._def;
            this._raw_vector = matrix._raw_vector;
            this.cols = matrix.cols;
            this.rows = matrix.rows;
        }

        public void Erase(int x, int y){
            this._raw_vector[x][y] = this._def;
        }

        public void Update(int x, int y, T v){
            this._raw_vector[x][y] = v;
        }

        public T At(int x, int y){
            return this._raw_vector[x][y];
        }

        public int Length(){
            return this._raw_vector.Length;
        }

        public void Clear(){
            T[] base_row= new T[this.cols];
            for(int i = 0; i < this.cols; ++i)
                base_row[i] = this._def;

            this._raw_vector = new T[this.rows][];
            for(int i = 0; i < this.rows; ++i)
                this._raw_vector[i] = base_row;

        }

        public Matrix<T> ShallowCopy(){
            return this;
        }

        public Matrix<T> DeepCopy(){
            return new Matrix<T>(this);
        }
    }


}