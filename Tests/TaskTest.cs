using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;

namespace UniversityRegistrar
{
  public class StudentTest : IDisposable
  {
    public StudentTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=university_registrar_test;Integrated Security=SSPI;";
    }

    [Fact]
    public void Test_EmptyAtFirst()
    {
      //Arrange, Act
      int result = Student.GetAll().Count;

      //Assert
      Assert.Equal(0, result);
    }

    [Fact]
    public void Test_EqualOverrideTrueForSameDescription()
    {
      //Arrange, Act
      Student firstStudent = new Student("Steve");
      Student secondStudent = new Student("Steve");

      //Assert
      Assert.Equal(firstStudent, secondStudent);
    }

    [Fact]
    public void Test_Save()
    {
      //Arrange
      Student testStudent = new Student("Steve");
      testStudent.Save();

      //Act
      List<Student> result = Student.GetAll();
      List<Student> testList = new List<Student>{testStudent};

      //Assert
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_SaveAssignsIdToObject()
    {
      //Arrange
      Student testStudent = new Student("Steve");
      testStudent.Save();

      //Act
      Student savedStudent = Student.GetAll()[0];

      int result = savedStudent.GetId();
      int testId = testStudent.GetId();

      //Assert
      Assert.Equal(testId, result);
    }

    [Fact]
    public void Test_FindFindsStudentInDatabase()
    {
      //Arrange
      Student testStudent = new Student("Steve");
      testStudent.Save();

      //Act
      Student result = Student.Find(testStudent.GetId());

      //Assert
      Assert.Equal(testStudent, result);
    }
    [Fact]
    public void Test_AddClass_AddsClassToStudent()
    {
      //Arrange
      Student testStudent = new Student("Steve");
      testStudent.Save();

      Class testClass = new Class("History");
      testClass.Save();

      //Act
      testStudent.AddClass(testClass);

      List<Class> result = testStudent.GetClasses();
      List<Class> testList = new List<Class>{testClass};

      //Assert
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_GetClasses_ReturnsAllStudentClasses()
    {
      //Arrange
      Student testStudent = new Student("Steve");
      testStudent.Save();

      Class testClass1 = new Class("History");
      testClass1.Save();

      Class testClass2 = new Class("Science");
      testClass2.Save();

      //Act
      testStudent.AddClass(testClass1);
      List<Class> result = testStudent.GetClasses();
      List<Class> testList = new List<Class> {testClass1};

      //Assert
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_Delete_DeletesStudentAssociationsFromDatabase()
    {
      //Arrange
      Class testClass = new Class("History");
      testClass.Save();

      string testDescription = "Steve";
      Student testStudent = new Student(testDescription);
      testStudent.Save();

      //Act
      testStudent.AddClass(testClass);
      testStudent.Delete();

      List<Student> resultClassStudents = testClass.GetStudents();
      List<Student> testClassStudents = new List<Student> {};

      //Assert
      Assert.Equal(testClassStudents, resultClassStudents);
    }


    [Fact]
    public void Test_Update_UpdatesInDb()
    {
      Student testStudent = new Student("Steve");
      testStudent.Save();
      testStudent.Update("Martin");

      Student newStudent = new Student("Martin", testStudent.GetId());

      Assert.Equal(testStudent, newStudent);
    }

    public void Dispose()
    {
      Student.DeleteAll();
      Class.DeleteAll();
    }
  }
}
