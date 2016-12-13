using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace UniversityRegistrar
{
  public class Class
  {
    private int _id;
    private string _name;

    public Class(string Name, int Id = 0)
    {
      _id = Id;
      _name = Name;
    }

    public override bool Equals(System.Object otherClass)
    {
        if (!(otherClass is Class))
        {
          return false;
        }
        else
        {
          Class newClass = (Class) otherClass;
          bool idEquality = this.GetId() == newClass.GetId();
          bool nameEquality = this.GetName() == newClass.GetName();
          return (idEquality && nameEquality);
        }
    }

    public int GetId()
    {
      return _id;
    }
    public string GetName()
    {
      return _name;
    }
    public void SetName(string newName)
    {
      _name = newName;
    }
    public static List<Class> GetAll()
    {
      List<Class> allClasses = new List<Class>{};

      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM classes;", conn);
      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int classId = rdr.GetInt32(0);
        string className = rdr.GetString(1);
        Class newClass = new Class(className, classId);
        allClasses.Add(newClass);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return allClasses;
    }

    public void Save()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO classes (name) OUTPUT INSERTED.id VALUES (@ClassName);", conn);

      SqlParameter nameParameter = new SqlParameter();
      nameParameter.ParameterName = "@ClassName";
      nameParameter.Value = this.GetName();
      cmd.Parameters.Add(nameParameter);
      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this._id = rdr.GetInt32(0);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if(conn != null)
      {
        conn.Close();
      }
    }

    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM classes;", conn);
      cmd.ExecuteNonQuery();
      conn.Close();
    }

    public void Delete()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM classes WHERE id = @ClassId; DELETE FROM classes_students WHERE class_id = @ClassId;", conn);

      SqlParameter catId = new SqlParameter();
      catId.ParameterName = "@ClassId";
      catId.Value = this.GetId();

      cmd.Parameters.Add(catId);
      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

    public static Class Find(int id)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM classes WHERE id = @ClassId;", conn);
      SqlParameter classIdParameter = new SqlParameter();
      classIdParameter.ParameterName = "@ClassId";
      classIdParameter.Value = id.ToString();
      cmd.Parameters.Add(classIdParameter);
      SqlDataReader rdr = cmd.ExecuteReader();

      int foundClassId = 0;
      string foundClassDescription = null;

      while(rdr.Read())
      {
        foundClassId = rdr.GetInt32(0);
        foundClassDescription = rdr.GetString(1);
      }
      Class foundClass = new Class(foundClassDescription, foundClassId);

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return foundClass;
    }

    public void AddStudent(Student newStudent)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO classes_students (class_id, student_id) VALUES (@ClassId, @StudentId);", conn);

      SqlParameter classIdParameter = new SqlParameter();
      classIdParameter.ParameterName = "@ClassId";
      classIdParameter.Value = this.GetId();
      cmd.Parameters.Add(classIdParameter);

      SqlParameter studentIdParameter = new SqlParameter();
      studentIdParameter.ParameterName = "@StudentId";
      studentIdParameter.Value = newStudent.GetId();
      cmd.Parameters.Add(studentIdParameter);

      cmd.ExecuteNonQuery();

      if(conn!=null)
      {
        conn.Close();
      }
    }

    public List<Student> GetStudents()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT students.* FROM classes JOIN classes_students ON (classes.id = classes_students.class_id) JOIN students ON (classes_students.student_id = students.id) WHERE classes.id = @ClassId;", conn);

      SqlParameter classIdParameter = new SqlParameter();
      classIdParameter.ParameterName = "@ClassId";
      classIdParameter.Value = this.GetId();
      cmd.Parameters.Add(classIdParameter);

      SqlDataReader rdr = cmd.ExecuteReader();

      List<int> studentIds = new List<int> {};
      while(rdr.Read())
      {
        int studentId = rdr.GetInt32(0);
        studentIds.Add(studentId);
      }
      if (rdr!=null)
      {
        rdr.Close();
      }

      List<Student> students = new List<Student> {};
      foreach (int studentId in studentIds)
      {
        SqlCommand studentQuery = new SqlCommand("SELECT * FROM students WHERE id = @StudentId;", conn);

        SqlParameter studentIdParameter = new SqlParameter();
        studentIdParameter.ParameterName = "@StudentId";
        studentIdParameter.Value = studentId;
        studentQuery.Parameters.Add(studentIdParameter);

        SqlDataReader queryReader = studentQuery.ExecuteReader();
        while(queryReader.Read())
        {
          int thisStudentId = queryReader.GetInt32(0);
          string studentDescription = queryReader.GetString(1);
          Student foundStudent = new Student(studentDescription, thisStudentId);
          students.Add(foundStudent);
        }
        if(queryReader!=null)
        {
          queryReader.Close();
        }
      }
      if(conn!=null)
      {
        conn.Close();
      }
      return students;
    }
  }
}
