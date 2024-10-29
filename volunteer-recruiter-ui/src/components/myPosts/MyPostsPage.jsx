import React, { useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import './MyPostsPage.css';
import Button from '../common/Button';
import { handleLogout, formatDate } from '../../utils/GeneralUtils';
import { navigateToHomePage } from '../../utils/NavigationUtils';
import { apiRequest, BASE_URL } from '../../utils/ApiUtils';

/**
 * `MyPostsPage` displays the user's posts with options to edit, delete, send mail, and view volunteers.
 * 
 * State:
 * - `posts` (array): List of posts to display.
 * - `showMailModal` (boolean): Controls visibility of the mail modal.
 * - `currentPostId` (string): ID of the post currently selected for email sending.
 * - `emailContent` (string): Content of the email message.
 * - `volunteers` (array): List of volunteers for a selected post.
 * - `showVolunteers` (boolean): Controls visibility of the volunteers list.
 * 
 * Functions:
 * - `handleDelete`: Deletes a specified post.
 * - `handleEdit`: Navigates to the edit page with the selected post data.
 * - `handleSendMail`: Sends an email to volunteers of a selected post.
 * - `handleViewVolunteers`: Retrieves and displays a list of volunteers for a post.
 */

const MyPostsPage = () => {

  const navigate = useNavigate();
  const location = useLocation();

  const [posts, setPosts] = useState(location.state?.posts || []);
  const [showMailModal, setShowMailModal] = useState(false);
  const [currentPostId, setCurrentPostId] = useState(null);
  const [emailContent, setEmailContent] = useState('');
  const [volunteers, setVolunteers] = useState([]);
  const [showVolunteers, setShowVolunteers] = useState(false);

  /**
   * Deletes a post by its ID and updates the post list.
   * @param {string} postId - ID of the post to delete.
   */
  const handleDelete = async (postId) => {
    try {
      await apiRequest(BASE_URL + '/deletepost', 'POST', {
        connectionKey: document.cookie.split('=')[1],
        id: postId,
      });
      alert('Post deleted successfully');
      setPosts(posts.filter((post) => post.Id !== postId));
    } catch (error) {
      alert('Error: ' + error.message);
    }
  };

  /**
   * Navigates to the edit page with the selected post.
   * @param {Object} post - The post object to edit.
   */
  const handleEdit = (post) => {
    navigate('/add-post', { state: { post } });
  };

  /**
   * Opens a new post creation page.
   */
  const handleAddPost = () => {
    navigate('/add-post');
  };

  /**
   * Redirects to the homepage.
   */
  const handleHome = async () => {
    navigateToHomePage(navigate);
  };

  /**
   * Sends an email to volunteers of the selected post.
   */
  const handleSendMail = async () => {
    try {
      const connectionKey = document.cookie.split('=')[1];
      const response = await apiRequest(`${BASE_URL}/sendmail`, 'POST', {
        connectionKey,
        id: currentPostId,
        content: emailContent,
      });

      const operationStatus = JSON.parse(response.operationStatus);
      alert(operationStatus.Message);

      setShowMailModal(false); // Close the modal after sending the email
      setEmailContent(''); // Clear the email content
    } catch (error) {
      alert('Error: ' + error.message);
    }
  };

  /**
   * Retrieves and displays volunteers for a selected post.
   * @param {string} postId - ID of the post to view volunteers.
   */
  const handleViewVolunteers = async (postId) => {
    try {
      const connectionKey = document.cookie.split('=')[1];
      const response = await apiRequest(`${BASE_URL}/viewvolunteers`, 'POST', {
        connectionKey,
        id: postId,
      });
  
      const operationStatus = JSON.parse(response.operationStatus);
      console.log('Response:', response);
  
      if (operationStatus.Code === 'Success') {
        // Map each volunteer to a string combining FirstName and LastName
        const volunteersList = response.volunteers.map(
          (volunteer) => `${volunteer.FirstName} ${volunteer.LastName}`
        );
        setVolunteers(volunteersList); // Update state with the list of volunteer names
        setShowVolunteers(true); // Show the list on the page
      } else {
        alert('Failed to retrieve volunteers');
      }
    } catch (error) {
      alert('Error: ' + error.message);
    }
  };

  return (
    <div className="my-posts-page">
      <Button className="logout-button" onClick={() => handleLogout(navigate)}>Logout</Button>

      <div className="posts-container">
        <h2>My Posts</h2>
        {posts.length > 0 ? (
          posts.map((post) => (
            <div key={post.Id} className="post-item">
              <h3>{post.Title}</h3>
              <p><strong>Description:</strong> {post.Description}</p>
              <p><strong>Volunteer Area:</strong> {post.Location}</p>
              <p><strong>Job:</strong> {post.Job}</p>
              <p><strong>Address:</strong> {post.Address}</p>
              <p><strong>Phone Number:</strong> {post.PhoneNumber}</p>
              <p><strong>Dates:</strong> {formatDate(post.InitialDate)} - {formatDate(post.LastDate)}</p>
              <Button className="edit-button" onClick={() => handleEdit(post)}>Edit</Button>
              <Button className="delete-button" onClick={() => handleDelete(post.Id)}>Delete</Button>
              <Button className="send-mail-button" onClick={() => { setCurrentPostId(post.Id); setShowMailModal(true); }}>Send Mail</Button>
              <Button className="view-volunteers-button" onClick={() => handleViewVolunteers(post.Id)}>View Volunteers</Button>
            </div>
          ))
        ) : (
          <p>No posts to display.</p>
        )}
      </div>

      {showMailModal && (
        <div className="mail-modal">
          <h4>Send Mail</h4>
          <textarea
            value={emailContent}
            onChange={(e) => setEmailContent(e.target.value)}
            placeholder="Enter email content"
            rows="5"
            cols="30"
          />
          <br />
          <Button className="send-button" onClick={handleSendMail}>Send</Button>
          <Button className="close-button" onClick={() => { setShowMailModal(false); setEmailContent(''); }}>Close</Button>
          </div>
        )}
      
      {showVolunteers && (
        <div className="volunteers-list">
          <h4>Volunteers:</h4>
          <ul>
            {volunteers.map((volunteer, index) => (
              <li key={index}>{volunteer}</li> // Render each volunteer's name
            ))}
          </ul>
        </div>
      )}

      <div className="action-buttons">
        <Button onClick={handleAddPost}>Add Post</Button>
        <Button onClick={handleHome}>Home</Button>
      </div>
    </div>
  );
};

export default MyPostsPage;
