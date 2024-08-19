using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Windows.Forms;

namespace Contact_Management_System
{
    public partial class Form1 : Form
    {
        private string connectionString = "Data Source=contacts.db;Version=3;";
        private List<Contact> contacts = new List<Contact>();

        public Form1()
        {
            InitializeComponent();
            InitializeDatabase();
            LoadContacts();
        }

        private void InitializeDatabase()
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "CREATE TABLE IF NOT EXISTS Contacts (Id INTEGER PRIMARY KEY, Name TEXT, PhoneNumber TEXT, Email TEXT, Address TEXT)";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Contact newContact = new Contact()
            {
                Name = txtName.Text,
                PhoneNumber = txtPhoneNumber.Text,
                Email = txtEmail.Text,
                Address = txtAddress.Text
            };
            AddContactToDatabase(newContact);
            MessageBox.Show("Contact added successfully!");
            ClearTextBoxes();
            LoadContacts();
        }

        private void AddContactToDatabase(Contact contact)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "INSERT INTO Contacts (Name, PhoneNumber, Email, Address) VALUES (@Name, @PhoneNumber, @Email, @Address)";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.Parameters.AddWithValue("@Name", contact.Name);
                command.Parameters.AddWithValue("@PhoneNumber", contact.PhoneNumber);
                command.Parameters.AddWithValue("@Email", contact.Email);
                command.Parameters.AddWithValue("@Address", contact.Address);
                command.ExecuteNonQuery();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (listBoxContacts.SelectedItem != null)
            {
                int index = listBoxContacts.SelectedIndex;
                Contact editedContact = contacts[index];
                editedContact.Name = txtName.Text;
                editedContact.PhoneNumber = txtPhoneNumber.Text;
                editedContact.Email = txtEmail.Text;
                editedContact.Address = txtAddress.Text;
                UpdateContactInDatabase(editedContact);
                MessageBox.Show("Contact edited successfully!");
                ClearTextBoxes();
                LoadContacts();
            }
            else
            {
                MessageBox.Show("Please select a contact to edit.");
            }
        }

        private void UpdateContactInDatabase(Contact contact)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "UPDATE Contacts SET Name = @Name, PhoneNumber = @PhoneNumber, Email = @Email, Address = @Address WHERE Id = @Id";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.Parameters.AddWithValue("@Name", contact.Name);
                command.Parameters.AddWithValue("@PhoneNumber", contact.PhoneNumber);
                command.Parameters.AddWithValue("@Email", contact.Email);
                command.Parameters.AddWithValue("@Address", contact.Address);
                command.Parameters.AddWithValue("@Id", contact.Id);
                command.ExecuteNonQuery();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (listBoxContacts.SelectedItem != null)
            {
                int index = listBoxContacts.SelectedIndex;
                Contact deletedContact = contacts[index];
                DeleteContactFromDatabase(deletedContact);
                contacts.RemoveAt(index);
                MessageBox.Show("Contact deleted successfully!");
                ClearTextBoxes();
                LoadContacts();
            }
            else
            {
                MessageBox.Show("Please select a contact to delete.");
            }
        }

        private void DeleteContactFromDatabase(Contact contact)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "DELETE FROM Contacts WHERE Id = @Id";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.Parameters.AddWithValue("@Id", contact.Id);
                command.ExecuteNonQuery();
            }
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text; // Assuming txtSearch is a TextBox where users input search terms
            listBoxContacts.Items.Clear(); // Clear existing items
            foreach (var contact in SearchContacts(searchTerm)) // Assuming SearchContacts() searches based on the input
            {
                listBoxContacts.Items.Add(contact);
            }
        }
        private void listBoxContacts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxContacts.SelectedItem != null)
            {
                var selectedContact = (Contact)listBoxContacts.SelectedItem; // Assuming Contact is your contact class
                                                                             // Display selected contact details
                txtName.Text = selectedContact.Name; // Assuming txtName is a TextBox for the contact's name
                txtPhoneNumber.Text = selectedContact.PhoneNumber; // Similarly for phone number
                                                                   // Add other fields as necessary
            }
        }


        private void btnDisplayAll_Click(object sender, EventArgs e)
        {
            // Example code to display all contacts
            listBoxContacts.Items.Clear(); // Clear existing items
            foreach (var contact in GetAllContacts()) // Assuming GetAllContacts() fetches contacts from your data source
            {
                listBoxContacts.Items.Add(contact);
            }
        }


        private void LoadContacts()
        {
            contacts.Clear();
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM Contacts";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Contact contact = new Contact()
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString(),
                        PhoneNumber = reader["PhoneNumber"].ToString(),
                        Email = reader["Email"].ToString(),
                        Address = reader["Address"].ToString()
                    };
                    contacts.Add(contact);
                }
            }
            listBoxContacts.DataSource = null;
            listBoxContacts.DataSource = contacts;
            listBoxContacts.DisplayMember = "Name";
        }

        private void ClearTextBoxes()
        {
            txtName.Clear();
            txtPhoneNumber.Clear();
            txtEmail.Clear();
            txtAddress.Clear();
        }
    }

    public class Contact
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
    }
}
