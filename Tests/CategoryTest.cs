using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;

namespace UniversityRegistrar
{
  public class ClassTest : IDisposable
  {
    public ClassTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=university_registrar_test;Integrated Security=SSPI;";
    }

    [Fact]
    public void Test_ClassesEmptyAtFirst()
    {
      //Arrange, Act
      int result = Class.GetAll().Count;

      //Assert
      Assert.Equal(0, result);
    }

    [Fact]
    public void Test_Equal_ReturnsTrueForSameName()
    {
      //Arrange, Act
      Class firstClass = new Class("History");
      Class secondClass = new Class("History");

      //Assert
      Assert.Equal(firstClass, secondClass);
    }

    [Fact]
    public void Test_Save_SavesClassToDatabase()
    {
      //Arrange
      Class testClass = new Class("History");
      testClass.Save();

      //Act
      List<Class> result = Class.GetAll();
      List<Class> testList = new List<Class>{testClass};

      //Assert
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_Save_AssignsIdToClassObject()
    {
      //Arrange
      Class testClass = new Class("History");
      testClass.Save();

      //Act
      Class savedClass = Class.GetAll()[0];

      int result = savedClass.GetId();
      int testId = testClass.GetId();

      //Assert
      Assert.Equal(testId, result);
    }

    [Fact]
    public void Test_Find_FindsClassInDatabase()
    {
      //Arrange
      Class testClass = new Class("History");
      testClass.Save();

      //Act
      Class foundClass = Class.Find(testClass.GetId());

      //Assert
      Assert.Equal(testClass, foundClass);
    }

    public void Dispose()
    {
      Student.DeleteAll();
      Class.DeleteAll();
    }
    [Fact]
    public void Test_Delete_DeletesClassFromDatabase()
    {
      //Arrange
      string name1 = "History";
      Class testClass1 = new Class(name1);
      testClass1.Save();

      string name2 = "Science";
      Class testClass2 = new Class(name2);
      testClass2.Save();

      //Act
      testClass1.Delete();
      List<Class> resultClasses = Class.GetAll();
      List<Class> testClassList = new List<Class> {testClass2};

      //Assert
      Assert.Equal(testClassList, resultClasses);
    }
    [Fact]
    public void Test_AddStudent_AddsStudentToClass()
    {
     //Arrange
      Class testClass = new Class("History");
      testClass.Save();

      Student testStudent = new Student("Steve");
      testStudent.Save();

      Student testStudent2 = new Student("Martin");
      testStudent2.Save();

     //Act
      testClass.AddStudent(testStudent);
      testClass.AddStudent(testStudent2);

      List<Student> result = testClass.GetStudents();
      List<Student> testList = new List<Student>{testStudent, testStudent2};

     //Assert
      Assert.Equal(testList, result);
    }
    [Fact]
    public void Test_GetStudents_ReturnsAllClassStudents()
    {
      //Arrange
      Class testClass = new Class("History");
      testClass.Save();

      Student testStudent1 = new Student("Steve");
      testStudent1.Save();

      Student testStudent2 = new Student("Martin");
      testStudent2.Save();

      //Act
      testClass.AddStudent(testStudent1);
      List<Student> savedStudents = testClass.GetStudents();
      List<Student> testList = new List<Student> {testStudent1};

      //Assert
      Assert.Equal(testList, savedStudents);
    }
    [Fact]
    public void Test_Delete_DeletesClassAssociationsFromDatabase()
    {
      //Arrange
      Student testStudent = new Student("Steve");
      testStudent.Save();

      string testName = "History";
      Class testClass = new Class(testName);
      testClass.Save();

      //Act
      testClass.AddStudent(testStudent);
      testClass.Delete();

      List<Class> resultStudentClasses = testStudent.GetClasses();
      List<Class> testStudentClasses = new List<Class> {};

      //Assert
      Assert.Equal(testStudentClasses, resultStudentClasses);
    }
  }
}
