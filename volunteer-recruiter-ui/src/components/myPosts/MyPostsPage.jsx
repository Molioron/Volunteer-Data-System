import React from 'react';
import { useNavigate } from 'react-router-dom';
import './MyPostsPage.css';
import Button from '../common/Button';
import { handleLogout, formatDate } from '../../utils/GeneralUtils';
import { navigateToHomePage } from '../../utils/NavigationUtils';
import { apiRequest, BASE_URL } from '../../utils/ApiUtils';

const MyPostsPage = () => {

  const navigate = useNavigate();
  const location = useLocation();


  // State to store posts, initialized with posts from location state or an empty array
  const [posts, setPosts] = React.useState(location.state?.posts || []);

  // Function to handle deleting a post
  const handleDelete = async (postId) => {
    try {
      await apiRequest(BASE_URL + '/deletepost', 'POST', {
        connectionKey: document.cookie.split('=')[1], // Retrieve connection key from cookies
        id: postId,  // Pass the postId to delete
      });
      alert('Post deleted successfully');

      // Update state to remove the deleted post
      setPosts(posts.filter((post) => post.Id !== postId));
    } catch (error) {
      alert('Error: ' + error.message);
    }
  };

  // Update state to remove the deleted post
  const handleEdit = (post) => {
    navigate('/add-post', { state: { post } }); // Navigate to 'Add Post' page with the post data for editing
  };

  // Function to handle adding a new post
  const handleAddPost = () => {
    navigate('/add-post'); // Navigate to 'Add Post' page
  };

  // Function to handle navigation to the home page
  const handleHome = async () => {
    navigateToHomePage(navigate); // Use the utility function to navigate to the home page
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
            </div>
          ))
        ) : (
          <p>No posts to display.</p>
        )}
      </div>
      <div className="action-buttons">
        <Button onClick={handleAddPost}>Add Post</Button>
        <Button onClick={handleHome}>Home</Button>
      </div>
    </div>
  );
};

export default MyPostsPage;
