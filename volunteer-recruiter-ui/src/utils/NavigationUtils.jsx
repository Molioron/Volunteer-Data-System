import { apiRequest, BASE_URL } from './ApiUtils';

/**
 * `navigateToHomePage` fetches all posts and navigates to the main page with posts in state.
 * @param {Function} navigate - Function to navigate to the homepage.
 */
export const navigateToHomePage = async (navigate) => {
    try {
      console.log('Request Body:', {
        volunteerAreas: [],
        jobTypes: [],
        initialDate: '',
        lastDate: '',
        dateFilterType: '',
      });

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
          connectionKey: document.cookie.split('=')[1],
        }) // Send an empty object to fetch all posts without filters
      });

      console.log('Response:', allPostsResponse);
  
      const allPostsResult = await allPostsResponse.json();
      const postsOperationStatus = JSON.parse(allPostsResult.operationStatus);
  
      if (postsOperationStatus.Code === 'Success') {
        const allPosts = JSON.parse(allPostsResult.posts);
        navigate('/main', { state: { posts: allPosts } });
      } else {
        alert('Failed to fetch posts');
      }
    } catch (error) {
      console.log('Error@@@:', error.message);
      alert('Error!!!!: ' + error.message);
    }
  };

/**
 * `fetchUserPosts` retrieves posts created by the user and navigates to the 'My Posts' page with posts in state.
 * @param {Function} navigate - Function to navigate to the 'My Posts' page.
 */
    export const fetchUserPosts = async (navigate) => {
      try {
        const result = await apiRequest(BASE_URL + '/getuserposts', 'POST', { connectionKey: document.cookie.split('=')[1] });
        const posts = JSON.parse(result.posts);
        navigate('/my-posts', { state: { posts } }); // Navigate to 'My Posts' page with posts in state
      } catch (error) {
        alert('Error: ' + error.message);
      }
    };