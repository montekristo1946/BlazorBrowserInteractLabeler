from enum import Enum

LabelPattern = {
    0:"None" ,
    1:"Box",
    2:"Polygon",
    3:"PolyLine",
    4:"Point"
}


class Label:
    def __init__(self, **kwargs):
        self.Id: int = -1
        self.NameLabel: str = ""


class Image:
    def __init__(self, **kwargs):
        self.Id: int = -1
        self.NameImages: str = ""


class Point:
    def __init__(self, **kwargs):
        self.X: float = kwargs.get("X")
        self.Y: float = kwargs.get("Y")
        self.PositionInGroup: int = kwargs.get("PositionInGroup")


class Annotation:
    def __init__(self, **kwargs):
        self.Id: int = kwargs.get("Id")
        self.LabelId: int = kwargs.get("LabelId")
        self.Points: [Point] = kwargs.get("Points")
        self.LabelPattern: str = kwargs.get("LabelPattern")
        self.ImageFrameId: int = kwargs.get("ImageFrameId")


class BILDto:
    def __init__(self, **kwargs):
        self.Labels: [Label] = kwargs.get("Labels")
        self.Images: [Image] = kwargs.get("Images")
        self.Annotations: [Annotation] = self._dict_to_obj(kwargs.get("Annotations"))

    def _dict_to_obj(self, annotations_dict):
        if type(annotations_dict) is not list:
            return annotations_dict

        ret_arr = []
        for annot_src in annotations_dict:
            if type(annot_src) is dict:
                point_dicts = annot_src["Points"]
                points = []
                for point_dic in point_dicts:
                    x = point_dic['X']
                    y = point_dic['Y']
                    if 'PositionInGroup' in point_dic:
                        positionInGroup = point_dic['PositionInGroup']
                    else:
                        positionInGroup = -1
                    points.append(Point(X=x, Y=y, PositionInGroup=positionInGroup))

                annot_src = Annotation(Id=annot_src['Id'],
                               LabelId=annot_src['LabelId'],
                               Points=points,
                               ImageFrameId=annot_src['ImageFrameId'],
                               LabelPattern=annot_src['LabelPattern'])
            ret_arr.append(annot_src)


        return ret_arr
