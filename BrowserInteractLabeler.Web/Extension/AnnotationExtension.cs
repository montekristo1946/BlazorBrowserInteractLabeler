using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Common.DTO;

namespace BrowserInteractLabeler.Web.Extension;

public static class AnnotationExtension
{
    public static Annotation[] CloneDeep(this IEnumerable<Annotation> annots)
    {
        return annots is null ? Array.Empty<Annotation>() : annots.Select(annotSrc => annotSrc.CloneDeep()).ToArray();
    }

    public static Annotation CloneDeep(this Annotation annotSrc)
    {
        var pointsNew = new List<PointD>();
        if (annotSrc.Points is not null)
            pointsNew = annotSrc.Points.Select(pointSrc => new PointD()
            {
                X = pointSrc.X,
                Y = pointSrc.Y,
                Id = pointSrc.Id,
                PositionInGroup = pointSrc.PositionInGroup,
                AnnotationId = pointSrc.AnnotationId
            }).ToList();

        return new Annotation()
        {
            Id = annotSrc.Id,
            ImageFrameId = annotSrc.ImageFrameId,
            LabelPattern = annotSrc.LabelPattern,
            LabelId = annotSrc.LabelId,
            State = annotSrc.State,
            Points = pointsNew
        };
    }

    public static bool Equality(this Annotation[] initialAnnots, Annotation[] comparable)
    {
        if (initialAnnots.Count() != comparable.Count())
            return false;

        bool ComparePoints(List<PointD>? pointsA, List<PointD>? pointsB)
        {
            if (pointsA is null && pointsB is null)
                return true;

            if (pointsA?.Count != pointsB?.Count)
                return false;

            return pointsA.All(pA =>
            {
                var res = pointsB.FirstOrDefault(pB => pB.Id == pA.Id && pB.X == pA.X && pB.Y == pA.Y);
                return res is not null;
            });
        }

        var resCompare = initialAnnots.All(dbAnnot =>
        {
            var res = comparable.FirstOrDefault(p => p.Id == dbAnnot.Id
                                                        && p.ImageFrameId == dbAnnot.ImageFrameId
                                                        && p.State == dbAnnot.State
                                                        && p.LabelId == dbAnnot.LabelId
                                                        && ComparePoints(p.Points, dbAnnot.Points));
            return res is not null;
        });

        return resCompare;

    }
}