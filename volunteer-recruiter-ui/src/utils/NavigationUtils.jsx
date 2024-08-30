import { apiRequest, BASE_URL } from './ApiUtils';

export const navigateToHomePage = async (navigate) => {
    try {
      // Fetch all posts after successful login/signup
      const allPostsResponse = await fetch(BASE_URL + '/getfilteredposts', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          volunteerAreas: [],
          jobTypes: [],
          initialDate: '',
          lastDate: '',
          dateFilterType: '',
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

    // Function to fetch the user's posts
    export const fetchUserPosts = async (navigate) => {
      try {
        const result = await apiRequest(BASE_URL + '/getuserposts', 'POST', { connectionKey: document.cookie.split('=')[1] });
        const posts = JSON.parse(result.posts);
        navigate('/my-posts', { state: { posts } }); // Navigate to 'My Posts' page with posts in state
      } catch (error) {
        alert('Error: ' + error.message);
      }
    };