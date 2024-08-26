import { useNavigate, useLocation } from 'react-router-dom';
import React, { useState, useEffect } from 'react';
import AddPostInputField from './AddPostInputField';
import './AddPostPage.css';

const AddPostPage = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const post = location.state?.post; // Access the post data if passed for editing

  const [formData, setFormData] = useState({
    connectionKey: '',
    title: '',
    description: '',
    address: '',
    volunteerArea: '',
    jobType: '',
    initialDate: '',
    lastDate: '',
  });

  useEffect(() => {
    if (post) {
      // If post is passed, prefill the form with post data
      setFormData({
        title: post.Title,
        description: post.Description,
        address: post.Address,
        volunteerArea: post.Location,
        jobType: post.Job,
        initialDate: post.InitialDate,
        lastDate: post.LastDate,
      });
    }
  }, [post]);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    const url = post ? 'http://localhost:9000/editpost' : 'http://localhost:9000/createpost';
    const connectionKey = document.cookie.split('=')[1];
    const updateformData = { ...formData, connectionKey: connectionKey, id: post?.Id };

    try {
      const response = await fetch(url, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(updateformData),
      });
      console.log('Response:', response);
      console.log(JSON.stringify(updateformData));
      const result = await response.json();
      handleResponse(result);
    } catch (error) {
      console.error('Error:', error);
      alert('Error: ' + error.message);
    }
  };

  const handleResponse = async (response) => {
    try {
      const operationStatus = JSON.parse(response.operationStatus);
      if (operationStatus.Code === 'Success') {
        alert(operationStatus.Message);

        // Fetch the user's posts after the update or create operation
        const connectionKey = document.cookie.split('=')[1];
        const postsResponse = await fetch('http://localhost:9000/getuserposts', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({ connectionKey: connectionKey }),
        });

        const postsResult = await postsResponse.json();
        const postsOperationStatus = JSON.parse(postsResult.operationStatus);

        if (postsOperationStatus.Code === 'Success') {
          const userPosts = JSON.parse(postsResult.posts);
          navigate('/my-posts', { state: { posts: userPosts } });
        } else {
          alert('Failed to fetch posts after update');
        }
      } else {
        alert('Error: ' + operationStatus.Message);
      }
    } catch (error) {
      console.error('Error:', error);
      alert('Error: ' + error.message);
    }
  };

  const handleMyPosts = async () => {
    try {
      const connectionKey = document.cookie.split('=')[1];
      const response = await fetch('http://localhost:9000/getuserposts', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ connectionKey: connectionKey }),
      });

      const result = await response.json();
      const operationStatus = JSON.parse(result.operationStatus);

      if (operationStatus.Code === 'Success') {
        const userPosts = JSON.parse(result.posts);
        navigate('/my-posts', { state: { posts: userPosts } });
      } else {
        alert('No posts found');
      }
    } catch (error) {
      console.error('Error:', error);
      alert('Error: ' + error.message);
    }
  };

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
        navigate('/my-posts');
      } else {
        alert('Error: ' + operationStatus.Message);
      }
    } catch (error) {
      console.error('Error:', error);
      alert('Error: ' + error.message);
    }
  };

  return (
    <div className="add-post-page">
      <button className="logout-button" onClick={handleLogout}>Logout</button>
      <div>
        <div className='add-post'>
          <AddPostInputField
            title={formData.title}
            description={formData.description}
            address={formData.address}
            volunteerArea={formData.volunteerArea}
            jobType={formData.jobType}
            initialDate={formData.initialDate}
            lastDate={formData.lastDate}
            onChange={handleInputChange}
          />
        </div>
      </div>
      <div className="add-post-action-button">
        <button onClick={handleMyPosts}>My Posts</button>
        <button onClick={handleSubmit}>{post ? 'Update Post' : 'Create Post'}</button>
      </div>
    </div>
  );
};

export default AddPostPage;
