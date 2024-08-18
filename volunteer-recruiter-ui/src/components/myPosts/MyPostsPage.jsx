import React from 'react';
import { useNavigate } from 'react-router-dom';
import './MyPostsPage.css';

const MyPostsPage = () => {
  const navigate = useNavigate();

  const handleLogout = async () => {
    try {
      const response = await fetch('http://localhost:9000/logout', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ connectionKey: document.cookie.split('=')[1] }),
      });

      console.log(JSON.stringify({ connectionKey: document.cookie.split('=')[1] })); // Log form data to the console
      const result = await response.json();
      if (result.operationStatus.Code === 'Success') {
        alert(result.operationStatus.Message);
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

  const handleAddPost = () => {
    // Logic for adding a post (can be implemented later)
    alert('Add Post functionality coming soon!');
  };

  const handleHome = () => {
    navigate('/main');
  };

  return (
    <div className="my-posts-page">
      <button className="logout-button" onClick={handleLogout}>Logout</button>
      <div className="header-buttons">
        <button className="add-post-button" onClick={handleAddPost}>Add Post</button>
        <button className="home-button" onClick={handleHome}>Home</button>
      </div>
      <div className="posts-container">
        {/* Posts will be displayed here */}
      </div>
    </div>
  );
};

export default MyPostsPage;
