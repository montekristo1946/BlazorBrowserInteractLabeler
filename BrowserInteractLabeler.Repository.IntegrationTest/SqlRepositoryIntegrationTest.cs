using AutoFixture;
using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Common.DTO;

namespace BrowserInteractLabeler.Repository.IntegrationTest;

public class SqlRepositoryIntegrationTest
{
    private readonly IRepository _operativeFramesStorage;
    private Fixture _fixture = new();

    public SqlRepositoryIntegrationTest(string pathDb)
    {
        _operativeFramesStorage = new SqlRepository();
        _operativeFramesStorage.LoadDatabaseAsync(pathDb).Wait();
    }

    public SqlRepositoryIntegrationTest()
    {
    }

    public void Dispose()
    {
        _operativeFramesStorage.Dispose();
    }

    public void InsertImageFramesTest()
    {
        var arrFrames = new List<ImageFrame>();

        for (int i = 0; i < 250; i++)
        {
            var sizeImage = _fixture.Build<SizeF>().With(p => p.Id, 0)
                .Create();
            var frame = _fixture.Build<ImageFrame>()
                .With(p => p.Id, 0)
                .With(p => p.SizeImage, sizeImage)
                .With(p => p.Annotations, new List<Annotation>())
                .Create();
            arrFrames.Add(frame);
        }


        var res = _operativeFramesStorage.InsertImageFramesAsync(arrFrames.ToArray()).Result;
        if (!res)
            throw new Exception("Fail InsertImageFrames");
    }

    public void InsertLabelsTest()
    {
        var frame = _fixture.Build<Label>()
            .With(p => p.Id, 0)
            .CreateMany(100).ToArray();

        var res = _operativeFramesStorage.InsertLabelsAsync(frame).Result;
        if (!res)
            throw new Exception("Fail InsertLabels");
    }

    public void SaveAnnotationsAsyncTest(int countAnnot = 5, int countImg = 250)
    {
        var rnd = new Random();
        var arrAnnotations = new List<Annotation>();
        for (int i = 1; i < countImg; i++)
        {
            ImageFrame images = null;
            for (int j = 0; j < countAnnot; j++)
            {
                var points = _fixture.Build<PointD>()
                    .With(p => p.Id, 0)
                    .With(p => p.AnnotationId, -1)
                    .With(p => p.Annot, new Annotation())
                    .CreateMany(50).ToList();
                var annotation = _fixture.Build<Annotation>()
                    .With(p => p.Id, 0)
                    .With(p => p.LabelId, rnd.Next(1, 100))
                    .With(p => p.ImageFrameId, i)
                    .With(p => p.Points, points)

                    .Create();
                arrAnnotations.Add(annotation);
            }
        }

        var res = _operativeFramesStorage.SaveAnnotationsAsync(arrAnnotations.ToArray()).Result;
        if (!res)
            throw new Exception("Fail SaveAnnotationsAsyncTest");
    }


    public void GetAnnotationsFromImgIdAsyncTest()
    {
        var imgId = 1;
        var arrAnnot = _operativeFramesStorage.GetAnnotationsFromImgIdAsync(imgId).Result;
        if (!arrAnnot.Any())
            throw new Exception("Fail GetAnnotationsFromImgIdAsyncTest");
    }

    public void DeleteAnnotationsAsyncTest()
    {
        var imgId = 100;

        var arrAnnot = _operativeFramesStorage.GetAnnotationsFromImgIdAsync(imgId).Result;

        if (!arrAnnot.Any())
            throw new Exception("Fail GetAnnotationsFromImgIdAsync");

        var res = _operativeFramesStorage.DeleteAnnotationsAsync(arrAnnot).Result;

        if (!res)
            throw new Exception("Fail DeleteAnnotationsAsync");
    }

    public void GetAllIMagesIndexAsyncTest()
    {
        var res = _operativeFramesStorage.GetAllIndexImagesAsync().Result;

        if (!res.Any())
            throw new Exception("Fail GetAllIMagesIndexAsync");
    }

    public void GetImagesByIndexAsyncTest()
    {
        var imgId = 100;
        var res = _operativeFramesStorage.GetImagesByIndexAsync(imgId).Result;

        if (!res.Images.Any())
            throw new Exception("Fail GetImagesByIndexAsync");
    }

    public void GetAllLabelsAsyncTest()
    {
        var res = _operativeFramesStorage.GetAllLabelsAsync().Result;

        if (!res.Any())
            throw new Exception("Fail GetAllLabelsAsyncTest");
    }




    public void FailWriteSqlAnnotation()
    {
        var imageFrameId = 1;
        var annot = new Annotation()
        {
            LabelPattern = TypeLabel.Box,
            ImageFrameId = imageFrameId,
            LabelId = 2,
            Points = new List<PointD>()
            {
                new() { X = 0.5f, Y = 0.6f }, new() { X = 0.7f, Y = 0.8f },
                new() { X = 0.9f, Y = 1.0f },
            },
            State = StateAnnot.Active
        };

        var res = _operativeFramesStorage.SaveAnnotationsAsync(new[] { annot }).Result;
        if (!res)
            throw new Exception("Fail SaveAnnotationsAsyncTest");
    }

    public void TestFailInitSql(string fullPathDb)
    {
        using IRepository operativeFramesStorage = new SqlRepository();
        var resInit = operativeFramesStorage.LoadDatabaseAsync(fullPathDb).Result;
        if (!resInit)
            throw new Exception("Fail TestInitSql");
    }




}