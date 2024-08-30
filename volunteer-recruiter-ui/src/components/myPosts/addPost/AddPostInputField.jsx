import React from 'react';
import InputField from '../../common/InputField'; 

const AddPostInputField = ({ title, description, address, volunteerArea, jobType, initialDate, lastDate, onChange }) => {
  return (
    <div className="add-post-input-fields">
      <InputField label="Title:" type="text" name="title" value={title} onChange={onChange} required />
      <InputField label="Description:" multiline name="description" value={description} onChange={onChange} required rows="10" />
      <InputField label="Address:" type="text" name="address" value={address} onChange={onChange} required />
      <InputField label="Area:" type="select" name="volunteerArea" value={volunteerArea} onChange={onChange} options={[
        { label: "Select Area", value: "" },
        { label: "North", value: "North" },
        { label: "South", value: "South" },
        { label: "Central", value: "Central" }
      ]} />
      <InputField label="Job Type:" type="select" name="jobType" value={jobType} onChange={onChange} options={[
        { label: "Select Job Type", value: "" },
        { label: "Agriculture", value: "Agriculture" },
        { label: "Cooking", value: "Cooking" },
        { label: "Transportation", value: "Transportation" },
        { label: "Animal Care", value: "AnimalCare" }
      ]} />
      <InputField label="Initial Date:" type="date" name="initialDate" value={initialDate} onChange={onChange} />
      <InputField label="End Date:" type="date" name="lastDate" value={lastDate} onChange={onChange} />
    </div>
  );
};

export default AddPostInputField;