export const BASE_URL = 'http://localhost:9000';


/**
 * `apiRequest` performs an API request using the specified URL, method, and optional body data.
 * 
 * @param {string} url - The endpoint to send the request to.
 * @param {string} [method='GET'] - HTTP method for the request (e.g., 'POST').
 * @param {Object} [body=null] - Optional body data for requests that require it.
 * @returns {Promise<Object>} - The response data parsed as JSON.
 * 
 * @throws Will throw an error if the operation status is not 'Success'.
 */
export const apiRequest = async (url, method = 'GET', body = null) => {
    try {

      // Headers to specify the content type of the request
      const headers = {
        'Content-Type': 'application/json',
      };
  
      // Options for the fetch request, including the HTTP method and headers
      const options = {
        method, // HTTP method (GET, POST, etc.)
        headers,
      };
  
      // If there is a body, stringify it and add it to the request options
      if (body) {
        options.body = JSON.stringify(body); 
        console.log('Request Body:', body);
      }
      
      // Perform the fetch request to the specified URL with the given options
      const response = await fetch(url, options);
      // Parse the response JSON
      const result = await response.json();
      // Extract and parse the operation status from the result
      const operationStatus = JSON.parse(result.operationStatus);
  
      // If the operation status is not 'Success', throw an error
      if (operationStatus.Code !== 'Success') {
        throw new Error(operationStatus.Message || 'Failed to perform the request');
      }
  
      return result;
    } catch (error) {
      console.error('API Error:', error);
      throw error;
    }
  };
  