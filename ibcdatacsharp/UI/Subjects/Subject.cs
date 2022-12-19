using ibcdatacsharp.Common;

namespace ibcdatacsharp.UI.Subjects
{
    public class Subject : BaseObject
    {
        public string? centroMedico
        {
            get { return GetValue<string?>("centroMedico"); }
            set { SetValue("centroMedico", value); }
        }
        public string? doctor
        {
            get { return GetValue<string?>("doctor"); }
            set { SetValue("doctor", value); }
        }
        public string? auxiliar
        {
            get { return GetValue<string?>("auxiliar"); }
            set { SetValue("auxiliar", value); }
        }
        public string nombre
        {
            get { return GetValue<string>("nombre"); }
            set { SetValue("nombre", value); }
        }
        public int? edad
        {
            get { return GetValue<int?>("edad"); }
            set { SetValue("edad", value); }
        }
        public float? altura
        {
            get { return GetValue<float?>("altura"); }
            set { SetValue("altura", value); }
        }
        public float? peso
        {
            get { return GetValue<float?>("peso"); }
            set { SetValue("peso", value); }
        }
        public float? longitudPie
        {
            get { return GetValue<float?>("longitudPie"); }
            set { SetValue("longitudPie", value); }
        }
        public float? alturaHombros
        {
            get { return GetValue<float?>("alturaHombros"); }
            set { SetValue("alturaHombros", value); }
        }
        public float? anchuraHombros
        {
            get { return GetValue<float?>("anchuraHombros"); }
            set { SetValue("anchuraHombros", value); }
        }
        public float? alturaCodo
        {
            get { return GetValue<float?>("alturaCodo"); }
            set { SetValue("alturaCodo", value); }
        }
        public float? anchuraCodo
        {
            get { return GetValue<float?>("anchuraCodo"); }
            set { SetValue("anchuraCodo", value); }
        }
        public float? alturaMuneca
        {
            get { return GetValue<float?>("alturaMuneca"); }
            set { SetValue("alturaMuneca", value); }
        }
        public float? anchuraMuneca
        {
            get { return GetValue<float?>("anchuraMuneca"); }
            set { SetValue("anchuraMuneca", value); }
        }
        public float? alturaBrazo
        {
            get { return GetValue<float?>("alturaBrazo"); }
            set { SetValue("alturaBrazo", value); }
        }
        public float? anchuraBrazo
        {
            get { return GetValue<float?>("anchuraBrazo"); }
            set { SetValue("anchuraBrazo", value); }
        }
        public float? alturaAntebrazo
        {
            get { return GetValue<float?>("alturaAntebrazo"); }
            set { SetValue("alturaAntebrazo", value); }
        }
        public float? anchuraAntebrazo
        {
            get { return GetValue<float?>("anchuraAntebrazo"); }
            set { SetValue("anchuraAntebrazo", value); }
        }
        public float? alturaCadera
        {
            get { return GetValue<float?>("alturaCadera"); }
            set { SetValue("alturaCadera", value); }
        }
        public float? anchuraCadera
        {
            get { return GetValue<float?>("anchuraCadera"); }
            set { SetValue("anchuraCadera", value); }
        }
        public float? alturaRodilla
        {
            get { return GetValue<float?>("alturaRodilla"); }
            set { SetValue("alturaRodilla", value); }
        }
        public float? anchuraRodilla
        {
            get { return GetValue<float?>("anchuraRodilla"); }
            set { SetValue("anchuraRodilla", value); }
        }
    }
}