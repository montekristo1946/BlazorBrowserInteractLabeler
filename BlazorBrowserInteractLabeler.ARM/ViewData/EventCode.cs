using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BlazorBrowserInteractLabeler.ARM.ViewData;

public enum EventCode
{
    [Display(Name = "Не заданно"), Description(nameof(None))]
    None = 0,

    [Display(Name = "Следущее изображение"), Description(nameof(GoNext))]
    GoNext = 1,

    [Display(Name = "Предыдущее изображение"), Description(nameof(GoBack))]
    GoBack = 2,

    [Display(Name = "Сохранить аннотацию"), Description(nameof(SaveAnnotation))]
    SaveAnnotation = 3,

    [Display(Name = "Отменить действие"), Description(nameof(UndoAction))]
    UndoAction = 4,

    [Display(Name = "Повторить действие"), Description(nameof(RedoAction))]
    RedoAction = 5,

    [Display(Name = "Создать аннотацию типа Box"), Description(nameof(InitAnnotationBox))]
    InitAnnotationBox = 6,

    [Display(Name = "Создать аннотацию типа Polygon"), Description(nameof(InitAnnotationPolygon))]
    InitAnnotationPolygon = 7,

    [Display(Name = "Создать аннотацию типа Polyline"), Description(nameof(InitAnnotationPolyline))]
    InitAnnotationPolyline = 8,

    [Display(Name = "Создать аннотацию типа Point"), Description(nameof(InitAnnotationPoint))]
    InitAnnotationPoint = 9,


    [Display(Name = "Подогнать изображение по размеру"), Description(nameof(MoveDefault))]
    MoveDefault = 10,

    [Display(Name = "Удалить активный annot"), Description(nameof(DeleteActiveAnnot))]
    DeleteActiveAnnot =11,

    [Display(Name = "Класс разметки 1"), Description(nameof(Label1))]
    Label1 = 12,

    [Display(Name = "Класс разметки 2"), Description(nameof(Label2))]
    Label2 = 13,

    [Display(Name = "Класс разметки 3"), Description(nameof(Label3))]
    Label3 = 14,

    [Display(Name = "Класс разметки 4"), Description(nameof(Label4))]
    Label4 = 15,

    [Display(Name = "Класс разметки 5"), Description(nameof(Label5))]
    Label5 = 16,

    [Display(Name = "Класс разметки 6"), Description(nameof(Label6))]
    Label6 =17,

    [Display(Name = "Класс разметки 7"), Description(nameof(Label7))]
    Label7 = 18,

    [Display(Name = "Класс разметки 8"), Description(nameof(Label8))]
    Label8 = 19,

    [Display(Name = "Класс разметки 9"), Description(nameof(Label9))]
    Label9 = 20,

    [Display(Name = "Класс разметки 10"), Description(nameof(Label10))]
    Label10 = 21,

    [Display(Name = "Класс разметки 11"), Description(nameof(Label11))]
    Label11 = 22,

    
}