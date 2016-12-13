using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace UniversityRegistrar
{
  public class Student
  {
    private int _id;
    private string _name;

    public Student(string Description, int Id = 0)
    {
      _id = Id;
      _name = Description;
    }

    public override bool Equals(System.Object otherStudent)
    {
        if (!(otherStudent is Student))
        {
          return false;
        }
        else {
          Student newStudent = (Student) otherStudent;
          bool idEquality = this.GetId() == newStudent.GetId();
          bool nameEquality = this.GetDescription() == newStudent.GetDescription();
          return (idEquality && nameEquality);
        }
    }

    public int GetId()
    {
      return _id;
    }
    public string GetDescription()
    {
      return _name;
    }
    public void SetDescription(string newDescription)
    {
      _name = newDescription;
    }

    public static List<Student> GetAll()
    {
      List<Student> AllStudents = new List<Student>{};

      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM students;", conn);
      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int studentId = rdr.GetInt32(0);
        string studentDescription = rdr.GetString(1);
        Student newStudent = new Student(studentDescription, studentId);
        AllStudents.Add(newStudent);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return AllStudents;
    }

    public void Update(string newDescription)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("UPDATE students SET name = @NewDescription OUTPUT INSERTED.name where id = @StudentId;", conn);

      SqlParameter descParam = new SqlParameter();
      descParam.ParameterName = "@NewDescription";
      descParam.Value = newDescription;


      SqlParameter idParam = new SqlParameter();
      idParam.ParameterName = "@StudentId";
      idParam.Value = this._id;

      cmd.Parameters.Add(descParam);
      cmd.Parameters.Add(idParam);

      SqlDataReader rdr = cmd.ExecuteReader();

      while (rdr.Read())
      {
        this._name = rdr.GetString(0);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
    }

    public void Save()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO students (name) OUTPUT INSERTED.id VALUES (@StudentDescription)", conn);

      SqlParameter nameParam = new SqlParameter();
      nameParam.ParameterName = "@StudentDescription";
      nameParam.Value = this.GetDescription();


      cmd.Parameters.Add(nameParam);

      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this._id = rdr.GetInt32(0);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
    }

    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM students;", conn);
      cmd.ExecuteNonQuery();
      conn.Close();
    }

    public static Student Find(int id)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM students WHERE id = @StudentId", conn);
      SqlParameter studentIdParameter = new SqlParameter();
      studentIdParameter.ParameterName = "@StudentId";
      studentIdParameter.Value = id.ToString();
      cmd.Parameters.Add(studentIdParameter);
      SqlDataReader rdr = cmd.ExecuteReader();

      int foundStudentId = 0;
      string foundStudentDescription = null;

      while(rdr.Read())
      {
        foundStudentId = rdr.GetInt32(0);
        foundStudentDescription = rdr.GetString(1);
      }
      Student foundStudent = new Student(foundStudentDescription, foundStudentId);

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return foundStudent;
    }

    public void Delete()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM students WHERE id = @StudentId; DELETE FROM classes_students WHERE student_id = @StudentId", conn);

      SqlParameter studentIdParameter = new SqlParameter();
      studentIdParameter.ParameterName = "@StudentId";
      studentIdParameter.Value = this.GetId();

      cmd.Parameters.Add(studentIdParameter);
      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

    public void AddClass(Class newClass)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO classes_students (class_id, student_id) VALUES (@ClassId, @StudentId);", conn);

      SqlParameter classIdParameter = new SqlParameter();
      classIdParameter.ParameterName = "@ClassId";
      classIdParameter.Value = newClass.GetId();
      cmd.Parameters.Add(classIdParameter);

      SqlParameter studentIdParameter = new SqlParameter();
      studentIdParameter.ParameterName = "@StudentId";
      studentIdParameter.Value = this.GetId();
      cmd.Parameters.Add(studentIdParameter);

      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

    public List<Class> GetClasses()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT class_id FROM classes_students WHERE student_id = @StudentId;", conn);

      SqlParameter studentIdParameter = new SqlParameter();
      studentIdParameter.ParameterName = "@StudentId";
      studentIdParameter.Value = this.GetId();
      cmd.Parameters.Add(studentIdParameter);

      SqlDataReader rdr = cmd.ExecuteReader();

      List<int> classIds = new List<int> {};

      while (rdr.Read())
      {
        int classId = rdr.GetInt32(0);
        classIds.Add(classId);
      }
      if (rdr != null)
      {
        rdr.Close();
      }

      List<Class> classes = new List<Class> {};

      foreach (int classId in classIds)
      {
        SqlCommand classQuery = new SqlCommand("SELECT * FROM classes WHERE id = @ClassId;", conn);

        SqlParameter classIdParameter = new SqlParameter();
        classIdParameter.ParameterName = "@ClassId";
        classIdParameter.Value = classId;
        classQuery.Parameters.Add(classIdParameter);

        SqlDataReader queryReader = classQuery.ExecuteReader();
        while (queryReader.Read())
        {
          int thisClassId = queryReader.GetInt32(0);
          string className = queryReader.GetString(1);
          Class foundClass = new Class(className, thisClassId);
          classes.Add(foundClass);
        }
        if (queryReader != null)
        {
          queryReader.Close();
        }
      }
      if (conn != null)
      {
        conn.Close();
      }
      return classes;
    }
  }
}
