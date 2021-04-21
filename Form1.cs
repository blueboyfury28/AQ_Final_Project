using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Michel_Grandin_ProjetFinal
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        DataTable dtStudentsData = new DataTable();
        int selectedIndex ;
        private bool isUpdate;

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // construire les colonnes 
            dtStudentsData.Columns.Add("StudentNo", typeof(int));
            dtStudentsData.Columns.Add("FirstName", typeof(string));
            dtStudentsData.Columns.Add("LastName", typeof(string));
            dtStudentsData.Columns.Add("CCode", typeof(string));
            dtStudentsData.Columns.Add("CNumber", typeof(int));
            dtStudentsData.Columns.Add("Title", typeof(string));
            dtStudentsData.Columns.Add("Note", typeof(string));
            //read les donne
            string[] lines = File.ReadAllLines("students.txt");
            foreach(string row in lines)
            {
                //les données de l'élève sont divisées par un séparateur pour obtenir le prénom, le nom et d'autres attributs
                string[] data = row.Split('|');
                dtStudentsData.Rows.Add(int.Parse(data[0]), data[1], data[2], data[3], int.Parse(data[4]), data[5],data[6]);
            }
            dataGridView1.DataSource = dtStudentsData;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            DataView dv = dtStudentsData.DefaultView;
            dv.RowFilter = string.Format("FirstName Like '%{0}%' OR LastName LIKE '%{1}%'", txtSearch.Text, txtSearch.Text);
            dataGridView1.DataSource = dv;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Student student = new Student();
            ClassLesson classLesson = new ClassLesson();
            ClassNote classNote = new ClassNote();
            int studentNo,courseNo;
            if(!int.TryParse(txtStudentNo.Text,out studentNo))
            {
                MessageBox.Show("Enter valid student number");
                return;
            }
            else if (!int.TryParse(txtCourseNo.Text, out courseNo))
            {
                MessageBox.Show("Enter valid course number");
                return;
            }
            student.StudentNumber = studentNo;
            student.FirstName = txtFirstName.Text;
            student.LastName = txtLastName.Text;

            classLesson.Coded = txtCourseCode.Text;
            classLesson.Coursenumber = courseNo;
            classLesson.Title = txtCourseTitle.Text;

            classNote.Coursenumber = courseNo;
            classNote.StudentNumber = student.StudentNumber;
            classNote.Note = txtNote.Text;

            //si la mise à jour est vraie, cela signifie que l'utilisateur souhaite mettre à jour les données d'étudiant sélectionnées
            if (isUpdate)
            {
                DataRow row = dtStudentsData.Rows[selectedIndex];
                row[0] = student.StudentNumber;
                row[1] = student.FirstName;
                row[2] = student.LastName;
                row[3] = classLesson.Coded;
                row[4] = classLesson.Coursenumber;
                row[5] = classLesson.Title;
                row[6] = classNote.Note;
                clearControls();
            }
            else
            { 
                //check if student alerady exists with same number
                foreach (DataRow row in dtStudentsData.Rows)
                {
                    if (int.Parse(row[0].ToString()) == student.StudentNumber)
                    {
                        MessageBox.Show("Student already exist try different student number");
                        return;
                    }
                }
                
                dtStudentsData.Rows.Add(student.StudentNumber, student.FirstName, student.LastName,
                    classLesson.Coded, classLesson.Coursenumber, classLesson.Title, classNote.Note);
                //refresh dataGrid
                dataGridView1.DataSource = dtStudentsData;
                clearControls();
                
               
            }
            saveDataToFile();
        }

        private void saveDataToFile()
        {
            string[] lines = new string[dtStudentsData.Rows.Count];
           for(int i=0;i<dtStudentsData.Rows.Count;i++)
            {
                DataRow row = dtStudentsData.Rows[i];
                //make each student in single line formate
                lines[i] = row[0] + "|" + row[1] + "|" + row[2] + "|" + row[3] + "|" + row[4] + "|" + row[5] + "|" + row[6];
            }
            File.WriteAllLines("students.txt", lines);
            dataGridView1.DataSource = dtStudentsData;
        }

        private void dataGridView1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
           
            // add to textboxes in order to edit the details
            if (e.RowIndex >= 0)
            {
                DataRow row = dtStudentsData.Rows[e.RowIndex];
                txtStudentNo.Text = row[0].ToString();
                txtFirstName.Text = row[1].ToString();
                txtLastName.Text = row[2].ToString();
                txtCourseCode.Text = row[3].ToString();
                txtCourseNo.Text = row[4].ToString();
                txtCourseTitle.Text = row[5].ToString();
                txtNote.Text = row[6].ToString();
                isUpdate = true;
                selectedIndex = e.RowIndex;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            clearControls();
        }

        private void clearControls()
        {
            isUpdate = false;
            txtStudentNo.Clear();
            txtFirstName.Clear();
            txtLastName.Clear();
            txtCourseCode.Clear();
            txtCourseNo.Clear();
            txtCourseTitle.Clear();
            txtNote.Clear();
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
         
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if ( dataGridView1.SelectedRows.Count >= 0)
            {
                int index = dataGridView1.SelectedRows[0].Index;
                foreach (DataRow row in dtStudentsData.Rows)
                {
                    if (int.Parse(row[0].ToString()) == int.Parse(dataGridView1.Rows[index].Cells[0].Value.ToString()))
                    {
                        dtStudentsData.Rows.RemoveAt(index);
                        saveDataToFile();
                        break;
                    }
                }
            }
        }
    }
}
