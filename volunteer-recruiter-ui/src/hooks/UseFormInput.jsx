import { useState } from 'react';

/**
 * `useFormInput` is a custom hook to manage form data state.
 * 
 * This hook provides a stateful `formData` object and a `handleInputChange` function 
 * to update the state based on form input changes.
 * 
 * Usage:
 * ```jsx
 * const { formData, handleInputChange, setFormData } = useFormInput({
 *    name: '',
 *    email: '',
 *    password: ''
 * });
 * ```
 * 
 * @param {Object} initialValues - Initial values for form fields.
 * @returns {Object} - An object containing:
 *   - `formData`: The state object holding form data.
 *   - `handleInputChange`: Function to handle input changes, updating `formData`.
 *   - `setFormData`: Function to directly set the `formData`.
 */

const useFormInput = (initialValues) => {
  const [formData, setFormData] = useState(initialValues);

  /**
   * Updates form data based on the event target's name and value.
   * @param {Event} e - The input change event.
   */
  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
  };
  

  return { formData, handleInputChange, setFormData };
};

export default useFormInput;