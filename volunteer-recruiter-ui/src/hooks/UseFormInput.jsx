import { useState } from 'react';

const useFormInput = (initialValues) => {
  const [formData, setFormData] = useState(initialValues);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
  };
  

  return { formData, handleInputChange, setFormData };
};

export default useFormInput;