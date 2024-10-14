using buoi_5;
using BUS;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace buoi_6___mo_hinh_3_lop
{
    public partial class Form1 : Form
    {
        private readonly StudentService studentService = new StudentService();
        public Form1()
        {
            InitializeComponent();
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;


        }

        public void Add()
        {
            try
            {
               
                string mssv = txbId.Text.Trim();
                string tenSinhVien = txbName.Text.Trim();

               
                if (string.IsNullOrEmpty(mssv) || string.IsNullOrEmpty(tenSinhVien))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin sinh viên.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DateTime ngaySinh = dateTimePicker1.Value; 
                int facultyId = (int)cbbFaculty.SelectedValue;

              
                StudentService studentService = new StudentService();
                bool isAdded = studentService.AddStudent(mssv, tenSinhVien, ngaySinh, facultyId); 

         
                if (isAdded)
                {
                    MessageBox.Show("Thêm sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                 
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Sinh viên đã tồn tại hoặc có lỗi xảy ra!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
              
                MessageBox.Show("Đã xảy ra lỗi trong quá trình thêm sinh viên: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void LoadData()
        {
         
            StudentService studentService = new StudentService();
            List<Student> students = studentService.GetAll();   

     
            FacultyService facultyService = new FacultyService();
            List<Faculty> faculties = facultyService.GetAll();

         
            var studentList = from student in students
                              join faculty in faculties on student.facultyId equals faculty.facultyId
                              select new
                              {
                                  mssv = student.mssv,
                                  tenSinhVien = student.tenSinhVien,
                                  ngaySinh = student.birthDate.ToString("dd/MM/yyyy"), 
                                  diemTB = student.diemTB,
                                  tenNganh = faculty.facultyName 
                              };

     
            dataGridView1.DataSource = studentList.ToList();

           
            dataGridView1.Columns["mssv"].HeaderText = "MSSV";
            dataGridView1.Columns["tenSinhVien"].HeaderText = "Tên Sinh Viên";
            dataGridView1.Columns["ngaySinh"].HeaderText = "Ngày Sinh";
            dataGridView1.Columns["tenNganh"].HeaderText = "Lớp";

          
            dataGridView1.Columns["diemTB"].Visible = false;

         
            cbbFaculty.DataSource = faculties;
            cbbFaculty.DisplayMember = "facultyName"; 
            cbbFaculty.ValueMember = "facultyId"; 
        }

        private void EnableSaveButtons(object sender, EventArgs e)
        {
           
            btnSave.Enabled = true;
            btnNoSave.Enabled = true;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            btnSave.Enabled = false;
            btnNoSave.Enabled = false;

            txbId.TextChanged += EnableSaveButtons;
            txbName.TextChanged += EnableSaveButtons;
            dateTimePicker1.ValueChanged += EnableSaveButtons;
            cbbFaculty.SelectedIndexChanged += EnableSaveButtons;
            LoadData(); 
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Add();
            Form1_Load(sender, e);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {

                string mssv = dataGridView1.SelectedRows[0].Cells["mssv"].Value.ToString();


                var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn xóa sinh viên này?",
                                                    "Xác nhận xóa",
                                                    MessageBoxButtons.YesNo);

                if (confirmResult == DialogResult.Yes)
                {

                    studentService.DeleteStudent(mssv);

                    MessageBox.Show("Xóa sinh viên thành công");


                    Form1_Load(sender, e);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn sinh viên cần xóa.");
            }
        }

      

       

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (Application.OpenForms["Form2"] == null)
            {
             
                Form2 registerMajorForm = new Form2();

              
                registerMajorForm.Show();
            }
            else
            {
           
                Application.OpenForms["Form2"].BringToFront();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Add();
            Form1_Load(sender, e);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {

                string mssv = dataGridView1.SelectedRows[0].Cells["mssv"].Value.ToString();


                var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn xóa sinh viên này?",
                                                    "Xác nhận xóa",
                                                    MessageBoxButtons.YesNo);

                if (confirmResult == DialogResult.Yes)
                {

                    studentService.DeleteStudent(mssv);

                    MessageBox.Show("Xóa sinh viên thành công");


                    Form1_Load(sender, e);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn sinh viên cần xóa.");
            }
        }

      



        private void btnUpdate_Click(object sender, EventArgs e)
        {
           
            string mssv = txbId.Text.Trim();
            string tenSinhVien = txbName.Text.Trim();
            int facultyId = (int)cbbFaculty.SelectedValue;

        
            DateTime birthDate = dateTimePicker1.Value;

          
            if (string.IsNullOrWhiteSpace(mssv) || string.IsNullOrWhiteSpace(tenSinhVien))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.");
                return;
            }

           
            bool isUpdated = studentService.UpdateStudent(mssv, tenSinhVien, birthDate, facultyId);

       
            if (isUpdated)
            {
                MessageBox.Show("Cập nhật thông tin sinh viên thành công");
                Form1_Load(sender, e); 
            }
            else
            {
                MessageBox.Show("Cập nhật thông tin sinh viên không thành công");
            }
        }



      

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Lấy hàng hiện tại được chọn
                DataGridViewRow row = dataGridView1.SelectedRows[0];

            
                txbId.Text = row.Cells["mssv"].Value.ToString();
                txbName.Text = row.Cells["tenSinhVien"].Value.ToString();

          
                if (row.Cells["ngaySinh"].Value != null)
                {
             
                    DateTime birthDate = Convert.ToDateTime(row.Cells["ngaySinh"].Value);
                    dateTimePicker1.Value = birthDate;
                }
                else
                {
                   
                    dateTimePicker1.Value = DateTime.Now;
                }

                // Lấy tên ngành (khoa) và gán vào ComboBox
                if (row.Cells["tenNganh"].Value != null)
                {
                    string tenNganh = row.Cells["tenNganh"].Value.ToString();
                  
                    cbbFaculty.SelectedIndex = cbbFaculty.FindStringExact(tenNganh);
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
           
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn thoát không?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

          
            if (result == DialogResult.Yes)
            {
             
                this.Close();
            }
          
        }


        private void btnNoSave_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];
                txbId.Text = row.Cells["mssv"].Value.ToString();
                txbName.Text = row.Cells["tenSinhVien"].Value.ToString();
                dateTimePicker1.Value = Convert.ToDateTime(row.Cells["ngaySinh"].Value);
                cbbFaculty.SelectedValue = row.Cells["facultyId"].Value;
            }

          
            btnSave.Enabled = false;
            btnNoSave.Enabled = false;
        }

        private void button2btnSave_Click(object sender, EventArgs e)
        {
            string mssv = txbId.Text.Trim();
            string tenSinhVien = txbName.Text.Trim();
            int facultyId = (int)cbbFaculty.SelectedValue;
            DateTime birthDate = dateTimePicker1.Value;

            if (string.IsNullOrWhiteSpace(mssv) || string.IsNullOrWhiteSpace(tenSinhVien))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.");
                return;
            }

            bool isUpdated = studentService.UpdateStudent(mssv, tenSinhVien, birthDate, facultyId);

            if (isUpdated)
            {
                MessageBox.Show("Thông tin sinh viên đã được lưu.");
                btnSave.Enabled = false;
                btnNoSave.Enabled = false;
                LoadData(); // Tải lại dữ liệu sau khi lưu
            }
            else
            {
                MessageBox.Show("Lưu thông tin không thành công.");
            }
        }

        private void SearchStudentByName(string searchName)
        {
            // Lấy danh sách sinh viên từ dịch vụ
            StudentService studentService = new StudentService();
            List<Student> students = studentService.GetAll();

            // Lấy danh sách các khoa từ dịch vụ
            FacultyService facultyService = new FacultyService();
            List<Faculty> faculties = facultyService.GetAll();

            // Lọc danh sách sinh viên theo tên sinh viên
            var filteredStudents = from student in students
                                   join faculty in faculties on student.facultyId equals faculty.facultyId
                                   where student.tenSinhVien.ToLower().Contains(searchName.ToLower()) // So sánh không phân biệt chữ hoa, thường
                                   select new
                                   {
                                       mssv = student.mssv,
                                       tenSinhVien = student.tenSinhVien,
                                       ngaySinh = student.birthDate.ToString("dd/MM/yyyy"), // Hiển thị ngày sinh
                                       diemTB = student.diemTB,
                                       tenNganh = faculty.facultyName // Thêm tên ngành (khoa)
                                   };

            // Gán danh sách tìm kiếm vào DataGridView
            dataGridView1.DataSource = filteredStudents.ToList();

            // Đổi tên các cột hiển thị
            dataGridView1.Columns["mssv"].HeaderText = "MSSV";
            dataGridView1.Columns["tenSinhVien"].HeaderText = "Tên Sinh Viên";
            dataGridView1.Columns["ngaySinh"].HeaderText = "Ngày Sinh";
            dataGridView1.Columns["tenNganh"].HeaderText = "Ngành";

            dataGridView1.Columns["diemTB"].Visible = false;

          
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
         
            string searchName = txbSearch.Text.Trim();

      
            SearchStudentByName(searchName);
        }

    }
}

