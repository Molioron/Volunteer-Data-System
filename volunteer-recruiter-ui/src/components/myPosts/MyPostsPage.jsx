import React from 'react';
import { useNavigate } from 'react-router-dom';
import './MyPostsPage.css';

const MyPostsPage = () => {
  const navigate = useNavigate();

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

  const handleAddPost = () => {
    navigate('/add-post');
  };

  const handleHome = () => {
    navigate('/main');
  };

  return (
    <div className="my-posts-page">
      <button className="logout-button" onClick={handleLogout}>Logout</button>
      <div className="posts-container">
        {/* Posts will be displayed here */}
      </div>
      <div className="action-buttons">
        <button onClick={handleAddPost}>Add Post</button>
        <button onClick={handleHome}>Home</button>
      </div>
    </div>
  );
};

export default MyPostsPage;
