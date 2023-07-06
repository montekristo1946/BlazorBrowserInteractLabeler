// See https://aka.ms/new-console-template for more information

using System.Threading.Channels;
using BrowserInteractLabeler.Repository.IntegrationTest;
Console.WriteLine("Run test");
// var pathRepo = "/mnt/Disk_D/TMP/20.05.2023/TMP/IMages/14_2023-04-26_13-09-37-635023.db3";

var pathRepo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{Path.GetRandomFileName()}.db3");
new SqlRepositoryIntegrationTest(pathRepo).InsertImageFramesTest();
new SqlRepositoryIntegrationTest(pathRepo).InsertLabelsTest();
new SqlRepositoryIntegrationTest(pathRepo).SaveAnnotationsAsyncTest();
new SqlRepositoryIntegrationTest(pathRepo).GetAnnotationsFromImgIdAsyncTest();
new SqlRepositoryIntegrationTest(pathRepo).DeleteAnnotationsAsyncTest();
new SqlRepositoryIntegrationTest(pathRepo).GetAllIMagesIndexAsyncTest();
new SqlRepositoryIntegrationTest(pathRepo).GetImagesByIndexAsyncTest();
new SqlRepositoryIntegrationTest(pathRepo).GetAllLabelsAsyncTest();
new SqlRepositoryIntegrationTest(pathRepo).GetLastIndexAnnotationTest();

// var pathDb= "/mnt/Disk_D/TMP/20.05.2023/TMP/IMages/14_2023-04-26_13-09-37-635023.db3";
// new SqlRepositoryIntegrationTest(pathDb).FailWriteSqlAnnotation();

Console.WriteLine("End test");