using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Reflection;
using Microsoft.Office.Interop.Excel;
using DataTable = System.Data.DataTable;

namespace tool.mysql.extend
{
    /// <summary>
    /// DataTable扩展方法类
    /// </summary>
    public static class DataTableExtend
    {
        /// <summary>
        /// DataTable转成List
        /// </summary>
        public static List<T> ToDataList<T>(this DataTable dt)
        {
            var list = new List<T>();
            var plist = new List<PropertyInfo>(typeof(T).GetProperties());

            if (dt == null || dt.Rows.Count == 0)
            {
                return list;
            }

            foreach (DataRow item in dt.Rows)
            {
                T s = Activator.CreateInstance<T>();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    PropertyInfo info = plist.Find(p => p.Name == dt.Columns[i].ColumnName);
                    if (info != null)
                    {
                        try
                        {
                            if (!Convert.IsDBNull(item[i]))
                            {
                                object v = null;
                                if (info.PropertyType.ToString().Contains("System.Nullable"))
                                {
                                    v = Convert.ChangeType(item[i], Nullable.GetUnderlyingType(info.PropertyType));
                                }
                                else
                                {
                                    v = Convert.ChangeType(item[i], info.PropertyType);
                                }
                                info.SetValue(s, v, null);
                            }
                        }
                        catch (Exception)
                        {
                            //LoggerHelper._.Error("字段[" + info.Name + "]转换出错", ex);
                            throw;
                        }
                    }
                }
                list.Add(s);
            }
            return list;
        }

        /// <summary>
        /// DataTable转成实体对象(单个)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static T ToDataEntity<T>(this DataTable dt)
        {
            T s = Activator.CreateInstance<T>();
            if (dt == null || dt.Rows.Count == 0)
            {
                return default(T);
            }
            var plist = new List<PropertyInfo>(typeof(T).GetProperties());
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                PropertyInfo info = plist.Find(p => p.Name == dt.Columns[i].ColumnName);
                if (info != null)
                {
                    try
                    {
                        if (!Convert.IsDBNull(dt.Rows[0][i]))
                        {
                            object v = null;
                            if (info.PropertyType.ToString().Contains("System.Nullable"))
                            {
                                v = Convert.ChangeType(dt.Rows[0][i], Nullable.GetUnderlyingType(info.PropertyType));
                            }
                            else
                            {
                                v = Convert.ChangeType(dt.Rows[0][i], info.PropertyType);
                            }
                            info.SetValue(s, v, null);
                        }
                    }
                    catch (Exception)
                    {
                        //LoggerHelper._.Error("字段[" + info.Name + "]转换出错", ex);
                        throw;
                    }
                }
            }
            return s;
        }

        /// <summary>
        /// List转成DataTable
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entities">实体集合</param>
        public static DataTable ToDataTable<T>(List<T> entities)
        {
            if (entities == null || entities.Count == 0)
            {
                return null;
            }

            var result = CreateTable<T>();
            FillData(result, entities);
            return result;
        }

        /// <summary>
        /// 创建表
        /// </summary>
        private static DataTable CreateTable<T>()
        {
            var result = new DataTable();
            var type = typeof(T);
            foreach (var property in type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
            {
                var propertyType = property.PropertyType;
                if ((propertyType.IsGenericType) && (propertyType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                    propertyType = propertyType.GetGenericArguments()[0];
                result.Columns.Add(property.Name, propertyType);
            }
            return result;
        }

        /// <summary>
        /// 填充数据
        /// </summary>
        private static void FillData<T>(DataTable dt, IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                dt.Rows.Add(CreateRow(dt, entity));
            }
        }

        /// <summary>
        /// 创建行
        /// </summary>
        private static DataRow CreateRow<T>(DataTable dt, T entity)
        {
            DataRow row = dt.NewRow();
            var type = typeof(T);
            foreach (var property in type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
            {
                row[property.Name] = property.GetValue(entity) ?? DBNull.Value;
            }
            return row;
        }



        #region Excel

        /// <summary>
        /// (DataGrid)导出为Excel文件
        /// </summary>
        /// <param name="dg"></param>
        //public static void SaveToExcelTwo(DataGrid dg)
        //{
        //    string fileName = "";
        //    string saveFileName = "";
            
        //    SaveFileDialog saveDialog = new SaveFileDialog
        //    {
        //        DefaultExt = "xlsx",
        //        Filter = "Excel 文件|*.xlsx",
        //        FileName = fileName
        //    };
        //    saveDialog.ShowDialog();
        //    saveFileName = saveDialog.FileName;
        //    if (saveFileName.IndexOf(":") < 0) return;  //被点了取消
        //    Application xlApp = new Application();
        //    if (xlApp == null)
        //    {
        //        System.Windows.MessageBox.Show("无法创建Excel对象，您可能未安装Excel");
        //        return;
        //    }
        //    Workbooks workbooks = xlApp.Workbooks;
        //    Workbook workbook = workbooks.Add(XlWBATemplate.xlWBATWorksheet);
        //    Worksheet worksheet = (Worksheet)workbook.Worksheets[1]; //取得sheet1

        //    //写入行
        //    for (int i = 0; i < dg.Columns.Count; i++)
        //    {
        //        worksheet.Cells[1, i + 1] = dg.Columns[i].Header;
        //    }
        //    for (int r = 0; r < dg.Items.Count; r++)
        //    {
        //        for (int i = 0; i < dg.Columns.Count; i++)
        //        {
        //            //if ((dg.Columns[i].GetCellContent(dg.Items[r]).ToString() == "System.Windows.Controls.ContentPresenter"))
        //            //{
        //            //    continue; // 按钮控件类型
        //            //}

        //            worksheet.Cells[r + 2, i + 1] = (dg.Columns[i].GetCellContent(dg.Items[r]) as TextBlock).Text;   //读取DataGrid某一行某一列的信息内容
        //        }
        //        System.Windows.Forms.Application.DoEvents();
        //    }
        //    worksheet.Columns.EntireColumn.AutoFit();
        //    System.Windows.MessageBox.Show(fileName + "保存成功");
        //    if (saveFileName != "")
        //    {
        //        try
        //        {
        //            workbook.Saved = true;
        //            workbook.SaveCopyAs(saveFileName);
        //        }
        //        catch (Exception ex)
        //        {
        //            System.Windows.MessageBox.Show("导出文件可能正在被打断!" + ex.Message);
        //        }
        //    }
        //    xlApp.Quit();
        //    GC.Collect();
        //}

        /// <summary>
        /// 根据excel文件路径，导入Excel文件转为 DataTable
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static DataTable TransformExcelToDataTable(string path)
        {
            try
            {
                //连接语句，读取文件路劲
                //string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=\"" + path + "\";" + "Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\"";
                string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source='{0}';Extended Properties='Excel 12.0;HDR=yes;IMEX=1'";

                strConn = string.Format(strConn, path);
                //查询Excel表名，默认是Sheet1
                string strExcel = "select * from [Sheet1$]";

                OleDbConnection ole = new OleDbConnection(strConn);
                ole.Open(); //打开连接

                //获取Excel工作薄中Sheet页(工作表)名集合
                DataTable ss = ole.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });

                //下面取得表名

                string strTableName = ss.Rows[0]["TABLE_NAME"].ToString();
                strTableName = strTableName.Substring(0, strTableName.IndexOf('$') + 1);
                OleDbDataAdapter da = new OleDbDataAdapter("SELECT * FROM [" + strTableName + "]", ole);
                DataSet ds = new DataSet();
                da.Fill(ds);
                da.Dispose();
                ole.Close();
                DataTable dt = ds.Tables[0];
                return dt;

                //DataTable dt = new DataTable();
                //OleDbDataAdapter odp = new OleDbDataAdapter(strExcel, strConn);

                //string sql_F = "Select * FROM [{0}]";
                //for (int i = 0; i < ss.Rows.Count; i++)
                //{
                //    odp.SelectCommand = new OleDbCommand(String.Format(sql_F, ss.Rows[i][2].ToString()), ole);
                //    odp.Fill(dt);
                //}

                ////odp.Fill(dt);
                //ole.Close();
                //return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
        
        /// <summary>
        /// 导出excel方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void SaveToExcel(SaveFileDialog saveFileDialog, DataTable dt)
        {
            Stream myStream = saveFileDialog.OpenFile();
            StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding("gb2312"));
            string str = "";

            //写标题
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (i > 0)
                {
                    str += "\t";
                }
                str += dt.Columns[i];
            }
            sw.WriteLine(str);
            //写内容
            for (int j = 0; j < dt.Rows.Count; j++)
            {
                string tempStr = "";
                for (int k = 0; k < dt.Columns.Count; k++)
                {
                    if (k > 0)
                    {
                        tempStr += "\t";
                    }
                    tempStr += dt.Rows[j][k].ToString();
                }
                sw.WriteLine(tempStr);

            }
            sw.Close();

            myStream.Close();

            //MessageBox.Show("导出成功");
        }
        


     }
}
