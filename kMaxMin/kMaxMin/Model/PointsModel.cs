using System;
using System.Collections.Generic;
using System.Windows.Media;
using kMaxMin.Model;

namespace kMaxMin.Model
{
    public class PointsModel
    {

        private Random random = new Random();

        private int pointsCount;

        public PointsModel(int pointsCount)
        {
            this.pointsCount = pointsCount;
        }


        public ClassModel GeneratePoints()
        {
            var firstClass = new ClassModel
            {
                ClassPoints = RandomPoints(),
                ClassColor = Color.FromRgb(GetRandomColorNumber(), GetRandomColorNumber(), GetRandomColorNumber()), 
                
            };
            firstClass.CentralPoints = firstClass.ClassPoints[random.Next(0, pointsCount)];

            return firstClass;
        }


        private byte GetRandomColorNumber()
        {
            return (byte)random.Next(0, 255);
        }


        private List<Points> RandomPoints()
        {
            var points = new List<Points>();
            for (int i = 0; i < pointsCount; i++)
            {
                points.Add(new Points
                {
                    XPoint = random.Next(0, 570),
                    YPoint = random.Next(0, 600)
                });
            }
            return points;
        }


        public Tuple<List<ClassModel>, bool> GenerateClasses(List<ClassModel> classes, List<Points> points)
        {
            var centerToPointLenght = new List<Points>();
            for (int i = 0; i < classes.Count; i++)
            {
                int pointIndex = ReturnIndexWithMaxLen(points, classes[i].CentralPoints);

                centerToPointLenght.Add(new Points
                {
                    XPoint = i,
                    YPoint = pointIndex
                });
            }
            if (classes.Count == 1)
            {
                var secondClass = new ClassModel
                {
                    CentralPoints = classes[centerToPointLenght[0].XPoint].ClassPoints[centerToPointLenght[0].YPoint],
                    ClassColor = Color.FromRgb(GetRandomColorNumber(), GetRandomColorNumber(), GetRandomColorNumber()),
                    ClassPoints = new List<Points>()
                };
                classes.Add(secondClass);
                PutPointsInToClasses(classes, points);
                return Tuple.Create(classes, true);
            }
            else
            {
                var result = GetNewLength(classes);
                if (IsNeedToBuildNew(classes, result.Item1))
                {

                    var newClass = new ClassModel();

                    newClass.CentralPoints = new Points
                    {
                        XPoint = classes[result.Item2.XPoint].ClassPoints[result.Item2.YPoint].XPoint,
                        YPoint = classes[result.Item2.XPoint].ClassPoints[result.Item2.YPoint].YPoint
                    };
                    newClass.ClassColor = Color.FromRgb(GetRandomColorNumber(), GetRandomColorNumber(), GetRandomColorNumber());
                    newClass.ClassPoints = new List<Points>();
                    classes.Add(newClass);
                    PutPointsInToClasses(classes, points);
                    return Tuple.Create(classes, true);
                }
                else
                {
                    return Tuple.Create(classes, false);
                }
            }
        }


        private void PutPointsInToClasses(List<ClassModel> classes, List<Points> points)
        {
            ClearClassList(classes);
            var centralPoints = CopyOldCenterPoints(classes);
            for (var i = 0; i < points.Count; i++)
            {
                var point = points[i];
                var index = ReturnPointIndex(centralPoints, point);
                classes[index].ClassPoints.Add(point);
            }
        }


        private List<Points> CopyOldCenterPoints(List<ClassModel> classes)
        {
            var newList = new List<Points>(classes.Count);
            foreach (var currentClass in classes)
            {
                newList.Add(currentClass.CentralPoints);
            }
            return newList;
        }


        private void ClearClassList(List<ClassModel> classes)
        {
            foreach (var currentClass in classes)
            {
                currentClass.ClassPoints.Clear();
            }
        }

        private Tuple<double, Points> GetNewLength(List<ClassModel> classModel)
        {
            double maxLen = 0;
            var point = new Points();
            for (int i = 0; i < classModel.Count; i++)
            {
                {
                    for (int j = 0; j < classModel[i].ClassPoints.Count; j++)
                    {
                        var newLength = CalculateLenght(classModel[i].ClassPoints[j], classModel[i].CentralPoints);
                        if (newLength > maxLen)
                        {
                            maxLen = newLength;
                            point.XPoint = i;
                            point.YPoint = j;
                        }
                    }
                }
            }
            return Tuple.Create(maxLen, point);
        }


        private bool IsNeedToBuildNew(List<ClassModel> classModels, double newCenterLenght)
        {
            double lenSum = 0;
            for (int i = 0; i < classModels.Count; i++)
            {
                if (i == classModels.Count)
                {
                    lenSum += CalculateLenght(classModels[i].CentralPoints, classModels[0].CentralPoints);
                }
                else
                {
                    if (i == 0)
                    {
                        lenSum += CalculateLenght(classModels[i].CentralPoints, classModels[i + 1].CentralPoints);
                    }
                    else
                    {
                        lenSum += CalculateLenght(classModels[i - 1].CentralPoints, classModels[i].CentralPoints);
                    }
                }
            }

            if (((lenSum / classModels.Count) /2) < newCenterLenght)
            {
                return true;
            }
            else
            {
               return false;
            }
        }

        private int ReturnPointIndex(List<Points> endPoints, Points start)
        {
            double minimalLenght = 0;
            int index = 0;
            for (int i = 0; i < endPoints.Count; i++)
            {
                if (i == 0)
                {
                    minimalLenght = CalculateLenght(endPoints[i], start);
                }
                else
                {
                    var newLenght = CalculateLenght(endPoints[i], start);
                    if (newLenght < minimalLenght)
                    {
                        minimalLenght = newLenght;
                        index = i;
                    }
                }
            }
            return index;
        }


        private int ReturnIndexWithMaxLen(List<Points> endPoints, Points start)
        {
            double maxLenght = 0;
            int index = 0;
            for (int i = 0; i < endPoints.Count; i++)
            {
                if (i == 0)
                {
                    maxLenght = CalculateLenght(endPoints[i], start);
                }
                else
                {
                    var newLenght = CalculateLenght(endPoints[i], start);
                    if (newLenght > maxLenght)
                    {
                        maxLenght = newLenght;
                        index = i;
                    }
                }
            }
            return index;
        }

        private double CalculateLenght(Points point, Points centerClassPoint)
        {
            var yLenght = Math.Abs(point.YPoint - centerClassPoint.YPoint);
            var xLenght = Math.Abs(point.XPoint - centerClassPoint.XPoint);
            return Math.Sqrt(Pow(xLenght) + Pow(yLenght));
        }


        private int Pow(int value)
        {
            return value * value;
        }
    }
}