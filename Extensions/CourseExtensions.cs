using CourseEnrollment.Models;

namespace CourseEnrollment.Extensions
{
    public static class CourseExtensions
    {
        public static decimal CalculateFinalFees(this Course course)
        {
            decimal discount =
                course.Fees * course.DiscountPercentage / 100m;

            return course.Fees - discount;
        }
    }
}