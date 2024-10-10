using BTBuoi5_2280603808_NguyenTranThienY.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace BTBuoi5_2280603808_NguyenTranThienY
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                StudentContextDB context = new StudentContextDB();
                List<Faculty> danhSachKhoa = context.Faculties.ToList();
                List<Student> danhSachSV = context.Students.ToList();
                var danhSachSVKHOA = danhSachSV
                        .Join(danhSachKhoa, sv => sv.FacultyID, k => k.FacultyID, (sv, k) => new SVKhoa
                        {
                            StudentID = sv.StudentID,
                            FullName = sv.FullName,
                            Faculty = k.FacultyName,
                            DiemTB = (float)sv.AverageScore
                        })
                        .ToList();
                    dataGridView1.DataSource = danhSachSVKHOA;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public class SVKhoa
        {
            public string StudentID { get; set; }
            public string FullName { get; set; }
            public string Faculty { get; set; }
            public float DiemTB { get; set; }
        }

        private void btnThem_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMSSV.Text) || string.IsNullOrEmpty(txtHoTen.Text) || cmbKhoa.SelectedItem == null || string.IsNullOrEmpty(txtDTB.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                    return;
                }
                if (txtMSSV.Text.Length != 10)
                {
                    MessageBox.Show("Mã số sinh viên phải có 10 kí tự!");
                    return;
                }
                StudentContextDB context = new StudentContextDB();
                if (context.Students.Any(sv => sv.StudentID == txtMSSV.Text))
                {
                      MessageBox.Show("Sinh viên đã tồn tại");
                      return;
                }
                if (!float.TryParse(txtDTB.Text, out float DiemTB))
                {
                      MessageBox.Show("Vui lòng nhập giá trị hợp lệ cho Điểm TB!");
                      return;
                }

                Faculty khoa = context.Faculties.FirstOrDefault(k => k.FacultyName == cmbKhoa.SelectedItem.ToString());
                if (khoa == null)
                {
                      MessageBox.Show("Không tìm thấy khoa!");
                      return;
                }
                Student sinhvien = new Student
                {
                       StudentID = txtMSSV.Text,
                       FullName = txtHoTen.Text,
                       FacultyID = khoa.FacultyID,
                       AverageScore  = DiemTB 
                };

                 context.Students.Add(sinhvien);
                 context.SaveChanges();
                 List<Student> danhSachSV = context.Students.ToList();
                 List<Faculty> danhSachKhoa = context.Faculties.ToList();
                 var DSSVKhoa = danhSachSV
                    .Join(danhSachKhoa, sv => sv.FacultyID, k => k.FacultyID, (sv, k) => new SVKhoa
                    {
                        StudentID = sv.StudentID,
                        FullName = sv.FullName,
                        Faculty = k.FacultyName,
                        DiemTB = (float)sv.AverageScore
                    })
                    .ToList();
                    dataGridView1.DataSource = DSSVKhoa;
                    MessageBox.Show("Thêm mới dữ liệu thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thêm sinh viên" + ex.Message);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra thông tin bắt buộc
                if (string.IsNullOrEmpty(txtHoTen.Text) || cmbKhoa.SelectedItem == null || string.IsNullOrEmpty(txtDTB.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                    return;
                }
                int rowIndex = dataGridView1.CurrentCell.RowIndex;
                string studentID = dataGridView1.Rows[rowIndex].Cells["StudentID"].Value.ToString();
                if (studentID.Length != 10)
                {
                    MessageBox.Show("Mã số sinh viên phải có 10 kí tự!");
                    return;
                }

                StudentContextDB context = new StudentContextDB();
                Faculty khoa = context.Faculties.FirstOrDefault(k => k.FacultyName == cmbKhoa.SelectedItem.ToString());
                if (khoa == null)
                {
                    MessageBox.Show("Không tìm thấy khoa!");
                    return;
                }

                if (!float.TryParse(txtDTB.Text, out float DiemTB))
                {
                    MessageBox.Show("Vui lòng nhập giá trị hợp lệ cho Điểm TB!");
                    return;
                }

                Student sinhvien = context.Students.Find(studentID);
                if (sinhvien == null)
                {
                    MessageBox.Show("Không tìm thấy MSSV cần sửa!");
                    return;
                }
                sinhvien.FullName = txtHoTen.Text;
                sinhvien.FacultyID = khoa.FacultyID;
                sinhvien.AverageScore = DiemTB;
                context.SaveChanges();

                LoadDataGridView();

                MessageBox.Show("Cập nhật dữ liệu thành công!");
                ResetForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi sửa sinh viên: " + ex.Message);
            }
        }
        private void LoadDataGridView()
        {
            using (StudentContextDB context = new StudentContextDB())
            {
                List<Faculty> danhSachKhoa = context.Faculties.ToList();
                List<Student> danhSachSV = context.Students.ToList();
                var danhSachSVKHOA = danhSachSV
                    .Join(danhSachKhoa, sv => sv.FacultyID, k => k.FacultyID, (sv, k) => new SVKhoa
                    {
                        StudentID = sv.StudentID,
                        FullName = sv.FullName,
                        Faculty = k.FacultyName,
                        DiemTB = (float)sv.AverageScore
                    })
                    .ToList();
                dataGridView1.DataSource = danhSachSVKHOA;
            }
        }
        private void ResetForm()
        {
            txtMSSV.Text = string.Empty;
            txtHoTen.Text = string.Empty;
            txtDTB.Text = string.Empty;
            cmbKhoa.SelectedIndex = -1; // hoặc chọn mục mặc định
        }
        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                int rowIndex = dataGridView1.CurrentCell.RowIndex;
                string studentID = dataGridView1.Rows[rowIndex].Cells["StudentID"].Value.ToString();
                StudentContextDB context = new StudentContextDB();
                Student sinhvien = context.Students.Find(studentID);
                if (sinhvien == null)
                {
                    MessageBox.Show("Không tìm thấy sinh viên cần xóa!");
                    return;
                }
                context.Students.Remove(sinhvien);
                context.SaveChanges();
                List<Student> danhSachSV = context.Students.ToList();
                List<Faculty> danhSachKhoa = context.Faculties.ToList();
                var DSSVKhoa = danhSachSV
                   .Join(danhSachKhoa, sv => sv.FacultyID, k => k.FacultyID, (sv, k) => new SVKhoa
                   {
                       StudentID = sv.StudentID,
                       FullName = sv.FullName,
                       Faculty = k.FacultyName,
                       DiemTB = (float)sv.AverageScore
                   })
                   .ToList();
                dataGridView1.DataSource = DSSVKhoa;
                MessageBox.Show("Xóa sinh viên thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xóa sinh viên" + ex.Message);
            }
        }

        private void btnDong_Click(object sender, EventArgs e)
        {
            DialogResult dia = MessageBox.Show("Bạn có muốn thoát chương trình?", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (dia == DialogResult.OK)
            {
                this.Close();
            }
        }
    }
}
