using BrowserInteractLabeler.Repository.IntegrationTest;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Is(LogEventLevel.Debug)
    .WriteTo.Console()
    .CreateLogger();

ILogger logger = Log.ForContext<Program>();

logger.Debug("Run test");
// var pathRepo = "/mnt/Disk_D/TMP/Hobbi/TMP/test.db3";

var pathRepo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{Path.GetRandomFileName()}.db3");
new SqlRepositoryIntegrationTest(pathRepo).InsertImageFramesTest();
new SqlRepositoryIntegrationTest(pathRepo).InsertLabelsTest();
new SqlRepositoryIntegrationTest(pathRepo).SaveAnnotationsAsyncTest();


new SqlRepositoryIntegrationTest(pathRepo).GetAnnotationsFromImgIdAsyncTest();
new SqlRepositoryIntegrationTest(pathRepo).DeleteAnnotationsAsyncTest();
new SqlRepositoryIntegrationTest(pathRepo).GetAllIMagesIndexAsyncTest();
new SqlRepositoryIntegrationTest(pathRepo).GetImagesByIndexAsyncTest();
new SqlRepositoryIntegrationTest(pathRepo).GetAllLabelsAsyncTest();

new SqlRepositoryIntegrationTest(pathRepo).FailWriteSqlAnnotation();
new SqlRepositoryIntegrationTest().TestFailInitSql(pathRepo);


logger.Debug("End test");