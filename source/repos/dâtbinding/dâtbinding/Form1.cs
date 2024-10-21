using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Entity;
using System.Xml.Linq;

namespace dâtbinding
{
    public partial class Form1 : Form
    {
        private int currentRowIndex = -1;
        private BindingSource sachBindingSource = new BindingSource();
        private Model1 db;
        public Form1()
        {
            InitializeComponent();
            db = new Model1();
        }
        public void LoadData(DataGridView dgv)
        {
            using (var db = new Model1())
            {
                var students = db.Students
                                 .Include(s => s.Major)  
                                 .Select(s => new
                                 {
                                     s.mssv,
                                     s.tenSinhVien,
                                     FacultyName = s.Faculty != null ? s.Faculty.facultyName : "Chưa có ngành",
                                     s.diemTB,
                                 }).ToList();

                dgv.DataSource = students;
            }
        }

        public void DataBinding(DataGridView dgv, TextBox txtMssv, TextBox txtTenSinhVien, TextBox txtDiemTB, TextBox txtFaculty)
        {
           
            txtMssv.DataBindings.Clear();
            txtMssv.DataBindings.Add("Text", dgv.DataSource, "Mssv");

            txtTenSinhVien.DataBindings.Clear();
            txtTenSinhVien.DataBindings.Add("Text", dgv.DataSource, "TenSinhVien");

            txtDiemTB.DataBindings.Clear();
            txtDiemTB.DataBindings.Add("Text", dgv.DataSource, "DiemTB");

            txtFaculty.DataBindings.Clear();
            txtFaculty.DataBindings.Add("Text", dgv.DataSource, "FacultyName"); // Binding với FacultyName
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData(dataGridView1);
            if (dataGridView1.Rows.Count > 0)
            {
                currentRowIndex = 0; // Đặt chỉ số hàng hiện tại về 0
                LoadStudentData(currentRowIndex); // Tải dữ liệu cho sinh viên đầu tiên
            }

            DataBinding(dataGridView1, txbId, txbName, txbAverage, txbFaculty);
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                if (currentRowIndex > 0)
                {
                    currentRowIndex--;
                    dataGridView1.Rows[currentRowIndex].Selected = true;
                    dataGridView1.FirstDisplayedScrollingRowIndex = currentRowIndex;
                    LoadStudentData(currentRowIndex); // Đảm bảo đây là cách gọi đúng
                }
            }
        }
        private void LoadStudentData(int rowIndex)
        {
            if (rowIndex >= 0 && rowIndex < dataGridView1.Rows.Count)
            {
                DataGridViewRow row = dataGridView1.Rows[rowIndex];
                txbId.Text = row.Cells["mssv"].Value.ToString();
                txbName.Text = row.Cells["tenSinhVien"].Value.ToString();
                txbAverage.Text = row.Cells["diemTB"].Value.ToString();
                txbFaculty.Text = row.Cells["FacultyName"].Value.ToString();
            }
        }
        private void btnNext_Click(object sender, EventArgs e)
        {
           
            if (dataGridView1.Rows.Count > 0)
            {
                // Kiểm tra nếu hàng hiện tại là hàng cuối
                if (currentRowIndex < dataGridView1.Rows.Count - 1)
                {
                    currentRowIndex++; // Tăng chỉ số hàng hiện tại
                    dataGridView1.Rows[currentRowIndex].Selected = true; // Chọn hàng hiện tại
                    dataGridView1.FirstDisplayedScrollingRowIndex = currentRowIndex; // Cuộn đến hàng hiện tại
                    LoadStudentData(currentRowIndex); // Tải dữ liệu sinh viên
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var db = new Model1())
            {
                // Tạo một đối tượng sinh viên mới
                var newStudent = new Student
                {
                    mssv = txbId.Text,
                    tenSinhVien = txbName.Text,
                    diemTB = Convert.ToDouble(txbAverage.Text),
                    // Đặt facultyId cho ngàn nếu cần
                    facultyId = db.Faculties.FirstOrDefault(f => f.facultyName == txbFaculty.Text)?.facultyId // Tìm kiếm facultyId dựa trên tên
                };

                // Thêm sinh viên vào cơ sở dữ liệu
                db.Students.Add(newStudent);
                db.SaveChanges();

                // Tải lại dữ liệu vào DataGridView
                LoadData(dataGridView1);
                MessageBox.Show("Thêm sinh viên thành công!");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            using (var db = new Model1())
            {
                // Lấy mã sinh viên từ TextBox
                var mssvToDelete = txbId.Text;

                // Tìm sinh viên cần xóa
                var student = db.Students.FirstOrDefault(s => s.mssv == mssvToDelete);
                if (student != null)
                {
                    db.Students.Remove(student);
                    db.SaveChanges();

                    // Tải lại dữ liệu vào DataGridView
                    LoadData(dataGridView1);
                    MessageBox.Show("Xóa sinh viên thành công!");
                }
                else
                {
                    MessageBox.Show("Không tìm thấy sinh viên với MSSV này.");
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            using (var db = new Model1())
            {
                // Lấy mã sinh viên từ TextBox
                var mssvToEdit = txbId.Text;

                // Tìm sinh viên cần sửa
                var student = db.Students.FirstOrDefault(s => s.mssv == mssvToEdit);
                if (student != null)
                {
                    student.tenSinhVien = txbName.Text;
                    student.diemTB = Convert.ToDouble(txbAverage.Text);
                    student.facultyId = db.Faculties.FirstOrDefault(f => f.facultyName == txbFaculty.Text)?.facultyId; // Cập nhật facultyId

                    db.SaveChanges();

                    // Tải lại dữ liệu vào DataGridView
                    LoadData(dataGridView1);
                    MessageBox.Show("Sửa thông tin sinh viên thành công!");
                }
                else
                {
                    MessageBox.Show("Không tìm thấy sinh viên với MSSV này.");
                }
            }
        }
    }
}
