#pragma once

#include <iostream>

namespace Treehopper {
    namespace Libraries {
        struct vector3_t {
            float x;
            float y;
            float z;
        };

        inline std::ostream& operator<<(std::ostream& os, const vector3_t& v) {
            return os << "[x: " << v.x << ", y: " << v.y << ", z: " << v.z << "]";
        }

        struct eularAngles_t {
            float yaw;
            float roll;
            float pitch;
        };

        inline std::ostream& operator<<(std::ostream& os, const eularAngles_t& v) {
            return os << "[yaw: " << v.yaw << ", roll: " << v.roll << ", pitch: " << v.pitch << "]";
        }

        struct quaternion_t {
            float w;
            float x;
            float y;
            float z;
        };

        inline std::ostream& operator<<(std::ostream& os, const quaternion_t& v) {
            return os << "[w: " << v.w << ", x: " << v.x << ", y: " << v.y << ", z: " << v.z << "]";
        }
    }
}
