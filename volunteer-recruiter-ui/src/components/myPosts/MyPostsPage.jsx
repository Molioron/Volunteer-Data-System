import React from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import './MyPostsPage.css';

const MyPostsPage = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const [posts, setPosts] = React.useState(location.state?.posts || []);

  const handleLogout = async () => {
    try {
      const connectionKey = document.cookie.split('=')[1];
      const response = await fetch('http://localhost:9000/logout', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ connectionKey: connectionKey }),
      });

      const result = await response.json();
      const operationStatus = JSON.parse(result.operationStatus);
      if (operationStatus.Code === 'Success') {
        alert(operationStatus.Message);
        document.cookie = "connectionKey=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
        navigate('/');
      } else {
        alert('Error: ' + result.operationStatus.Message);
      }
    } catch (error) {
      console.error('Error:', error);
      alert('Error: ' + error.message);
    }
  };

  const handleDelete = async (postId) => {
    try {
      const connectionKey = document.cookie.split('=')[1];
      const response = await fetch('http://localhost:9000/deletepost', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ connectionKey: connectionKey, id: postId }),
      });

      const result = await response.json();
      const operationStatus = JSON.parse(result.operationStatus);

      if (operationStatus.Code === 'Success') {
        alert('Post deleted successfully');
        setPosts(posts.filter(post => post.Id !== postId));
      } else {
        alert('Failed to delete the post');
      }
    } catch (error) {
      console.error('Error:', error);
      alert('Error: ' + error.message);
    }
  };

  const handleEdit = (post) => {
    navigate('/add-post', { state: { post } });
  };

  const handleAddPost = () => {
    navigate('/add-post');
  };

  const handleHome = async () => {
    try {
      const allPostsResponse = await fetch('http://localhost:9000/getfilteredposts', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          volunteerAreas: [],
          jobTypes: [],
          initialDate: '',
          lastDate: '',
          dateFilterType: ''
        }) // Send an empty object to fetch all posts without filters
      });

      const allPostsResult = await allPostsResponse.json();
      const postsOperationStatus = JSON.parse(allPostsResult.operationStatus);

      if (postsOperationStatus.Code === 'Success') {
        const allPosts = JSON.parse(allPostsResult.posts);
        navigate('/main', { state: { posts: allPosts } });
      } else {
        alert('Failed to fetch posts');
      }
    } catch (error) {
      console.error('Error:', error);
      alert('Error: ' + error.message);
    }
  };

  const formatDate = (dateString) => {
    let [year, month, day] = dateString.split('-');
    day = day.split('T')[0];
    day = day.length === 1 ? `0${day}` : day;
    return `${day}.${month}.${year}`;
  };

  return (
    <div className="my-posts-page">
      <button className="logout-button" onClick={handleLogout}>Logout</button>
      <div className="posts-container">
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
              <button className="edit-button" onClick={() => handleEdit(post)}>Edit</button>
              <button className="delete-button" onClick={() => handleDelete(post.Id)}>Delete</button>
            </div>
          ))
        ) : (
          <p>No posts to display.</p>
        )}
      </div>
      <div className="action-buttons">
        <button onClick={handleAddPost}>Add Post</button>
        <button onClick={handleHome}>Home</button>
      </div>
    </div>
  );
};

export default MyPostsPage;
